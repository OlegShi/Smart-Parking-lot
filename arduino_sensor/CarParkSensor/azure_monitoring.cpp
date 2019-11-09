
// Azure include with connection string in azure_monitoring.h
#include "azure_monitoring.h"

#include <AzureIoTHub.h>
#if defined(IOT_CONFIG_MQTT)
    #include <AzureIoTProtocol_MQTT.h>
#elif defined(IOT_CONFIG_HTTP)
    #include <AzureIoTProtocol_HTTP.h>
#endif

#include <SoftwareSerial.h>
#include "TFMini.h"
#include <ESP8266WiFi.h> 

/* CODEFIRST_OK is the new name for IOT_AGENT_OK. The follow #ifndef helps during the name migration. Remove it when the migration ends. */
#ifndef  IOT_AGENT_OK
#define  IOT_AGENT_OK CODEFIRST_OK
#endif // ! IOT_AGENT_OK

#define MAX_DEVICE_ID_SIZE  20
extern TFMini tfmini;
extern SoftwareSerial mySerial;

int isParkBusy = 0;

// Functions AZURE
static void sendMessage(IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle, const unsigned char* buffer, size_t size)
{
    IOTHUB_MESSAGE_HANDLE messageHandle = IoTHubMessage_CreateFromByteArray(buffer, size);
    if (messageHandle == NULL)
    {
        LogInfo("unable to create a new IoTHubMessage\r\n");
    }
    else
    {
        if (IoTHubClient_LL_SendEventAsync(iotHubClientHandle, messageHandle, NULL, NULL) != IOTHUB_CLIENT_OK)
        {
            LogInfo("failed to hand over the message to IoTHubClient");
        }
        else
        {
            LogInfo("IoTHubClient accepted the message for delivery\r\n");
        }

        IoTHubMessage_Destroy(messageHandle);
    }
//    free((void*)buffer);
}

/*this function "links" IoTHub to the serialization library*/
static IOTHUBMESSAGE_DISPOSITION_RESULT IoTHubMessage(IOTHUB_MESSAGE_HANDLE message, void* userContextCallback)
{
    IOTHUBMESSAGE_DISPOSITION_RESULT result;
    const unsigned char* buffer;
    size_t size;
    if (IoTHubMessage_GetByteArray(message, &buffer, &size) != IOTHUB_MESSAGE_OK)
    {
        LogInfo("unable to IoTHubMessage_GetByteArray\r\n");
        result = IOTHUBMESSAGE_ABANDONED;
    }
    else
    {
        /*buffer is not zero terminated*/
        char* temp = (char *)malloc(size + 1);
        if (temp == NULL)
        {
            LogInfo("failed to malloc\r\n");
            result = IOTHUBMESSAGE_ABANDONED;
        }
        else
        {
            result = IOTHUBMESSAGE_ABANDONED;
            /*
            EXECUTE_COMMAND_RESULT executeCommandResult;

            memcpy(temp, buffer, size);
            temp[size] = '\0';
            executeCommandResult = EXECUTE_COMMAND(userContextCallback, temp);
            result =
                (executeCommandResult == EXECUTE_COMMAND_ERROR) ? IOTHUBMESSAGE_ABANDONED :
                (executeCommandResult == EXECUTE_COMMAND_SUCCESS) ? IOTHUBMESSAGE_ACCEPTED :
                IOTHUBMESSAGE_REJECTED;
               
            free(temp);
            */
        }
    }
    return result;
}

// Request sensor distance to TFMini and send message if need 
void checkSensorInformations(IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle)
{
    static int wasDistancePreviousNear = -1;
    static int previousDistanceNear = 0;
    static int previousDistanceFar = 0;
    int Distance = 0;
    int Strength = 0;
    
    // Take one TF Mini distance measurement, if the pins are connected
    if (mySerial.available())
    {
        Distance = tfmini.getDistance();
        Strength = tfmini.getRecentSignalStrength();
    }
    else {
        LogInfo("TFMini not Available"); 
        return;    
    }

    if (Distance >= SENSOR_MIN_LIMIT_VALUE && Distance < SENSOR_MAX_LIMIT_VALUE)
    {
        previousDistanceNear++; previousDistanceFar = 0;
        if (previousDistanceNear > SENSOR_MAX_RETRY_BEFORE_STABLE_VALUE) previousDistanceNear = SENSOR_MAX_RETRY_BEFORE_STABLE_VALUE+1;
    }
    else if (Distance < SENSOR_MIN_LIMIT_VALUE) // not accepted value
        return;
    else { 
        previousDistanceFar++; previousDistanceNear = 0; 
        if (previousDistanceFar > SENSOR_MAX_RETRY_BEFORE_STABLE_VALUE) previousDistanceFar = SENSOR_MAX_RETRY_BEFORE_STABLE_VALUE+1;
      }

    bool fMsgToSend = false;
    if (previousDistanceNear >= SENSOR_MAX_RETRY_BEFORE_STABLE_VALUE && wasDistancePreviousNear != 1)
    {
        wasDistancePreviousNear = 1; 
        fMsgToSend = true; 
        previousDistanceNear = previousDistanceFar = 0;
    }
    else if (previousDistanceFar >= SENSOR_MAX_RETRY_BEFORE_STABLE_VALUE && wasDistancePreviousNear != 0)
    {
        wasDistancePreviousNear = 0; 
        fMsgToSend = true; 
        previousDistanceNear = previousDistanceFar = 0;
    }

    LogInfo("Distance : %d cm, Near = %d, Far = %d, PreviousNear = %d", Distance, previousDistanceNear, previousDistanceFar, wasDistancePreviousNear);
    
    if (fMsgToSend)
    {
        int i=0;
        LogInfo(" =======> Sending sensor value Distance = %d cm, Strength = %d", Distance, Strength);
        char messageToIotHub[] = IOT_DEVICE_PRESENCE_JSON_STRING;
        for (i=0; i < strlen(messageToIotHub); i++)
          if (messageToIotHub[i] == '#') messageToIotHub[i] = (wasDistancePreviousNear == 0)? '0':'1';
        isParkBusy = (wasDistancePreviousNear == 0)? 0:1;
        // Put in comment to don"t send 
        sendMessage(iotHubClientHandle, (const unsigned char*)messageToIotHub, strlen(messageToIotHub));
        for (i=0; i < 6; i++)
        {
          digitalWrite(0, HIGH);   // turn the LED RED on 
          delay(200);
          digitalWrite(0, LOW);   // turn the LED RED off 
          delay(200);
        }
    }
}

// MAIN LOOP for AZURE
void azure_monitoring_run(void)
{
    srand((unsigned int)time(NULL));
    IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle;

#if defined(IOT_CONFIG_MQTT)
    iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(IOT_CONFIG_CONNECTION_STRING, MQTT_Protocol);
#elif defined(IOT_CONFIG_HTTP)
    iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(IOT_CONFIG_CONNECTION_STRING, HTTP_Protocol);
#else
    iotHubClientHandle = NULL;
#endif

    if (iotHubClientHandle == NULL)
    {
        LogInfo("Failed on IoTHubClient_CreateFromConnectionString\r\n");
    }
    else
    {
#ifdef MBED_BUILD_TIMESTAMP
        // For mbed add the certificate information
        if (IoTHubClient_LL_SetOption(iotHubClientHandle, "TrustedCerts", certificates) != IOTHUB_CLIENT_OK)
        {
            LogInfo("failure to set option \"TrustedCerts\"\r\n");
        }
#endif // MBED_BUILD_TIMESTAMP

        STRING_HANDLE commandsMetadata;
        char object[] = "test";
        
        if (IoTHubClient_LL_SetMessageCallback(iotHubClientHandle, IoTHubMessage, object) != IOTHUB_CLIENT_OK)
        {
            LogInfo("unable to IoTHubClient_SetMessageCallback\r\n");
        }
        else
        {
            while (1)
            {
                int fStatusOk = (WiFi.status() == WL_CONNECTED) && (mySerial.available())? 1:0;
                digitalWrite(2, HIGH);   // Blink Blue LED to indicate programm is alive 
                if (fStatusOk == 1) // Turn the RED Led on if pb with Wifi or TFMini, RED LED looks inverted
                  digitalWrite(0, HIGH); // Led off
                else digitalWrite(0, LOW); // Led on
    
                // CheckSensorInformations and sendMessage if needed
                checkSensorInformations(iotHubClientHandle);
                
                IoTHubClient_LL_DoWork(iotHubClientHandle);
                ThreadAPI_Sleep(500);
                digitalWrite(2, LOW);    // turn the LED off by making the voltage LOW
                if (fStatusOk == 1) 
                {
                  if (isParkBusy == 0)
                    digitalWrite(0, HIGH); // Led off
                  else digitalWrite(0, LOW); // Blink                  
                }
                ThreadAPI_Sleep(500);
            }
        }
        IoTHubClient_LL_Destroy(iotHubClientHandle);
    }
}
