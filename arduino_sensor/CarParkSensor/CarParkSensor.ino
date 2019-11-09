/*
Example code for Benewake TFMini time-of-flight distance sensor. 
by Peter Jansen (December 11/2017)
This example code is in the public domain.

This example communicates to the TFMini using a SoftwareSerial port at 115200, 
while communicating the distance results through the default Arduino hardware
Serial debug port. 

SoftwareSerial for some boards can be unreliable at high speeds (such as 115200). 
The driver includes some limited error detection and automatic retries, that
means it can generally work with SoftwareSerial on (for example) an UNO without
the end-user noticing many communications glitches, as long as a constant refresh
rate is not required. 

The (UNO) circuit:
 * Uno RX is digital pin 10 (connect to TX of TF Mini)
 * Uno TX is digital pin 11 (connect to RX of TF Mini)

THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE AUTHOR(S) BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.  
*/

#include <SoftwareSerial.h>
#include "TFMini.h"

// Azure include
#include "azure_monitoring.h"
#include <AzureIoTHub.h>
#if defined(IOT_CONFIG_MQTT)
    #include <AzureIoTProtocol_MQTT.h>
#elif defined(IOT_CONFIG_HTTP)
    #include <AzureIoTProtocol_HTTP.h>
#endif

// for Wifi set up
#include <ESP8266WiFi.h> 
#include "esp8266/sample_init.h"

// azure main loop (so no need to loop in loop() function
extern void azure_monitoring_run(void);

// Setup software serial port 
SoftwareSerial mySerial(13, 15);      // Uno RX (TFMINI TX), Uno TX (TFMINI RX)
TFMini tfmini;

// ************** WIFI connection to change depending of the place
static char ssid[] = "Isabel";
static char pass[] = "0143644547";
// ************** Azure connection string to change in azure_monitoring.h

// Card Setup
void setup() {
  // Initialize hardware serial port (serial debug port)
  Serial.begin(115200);
  // wait for serial port to connect. Needed for native USB port only
  while (!Serial);
  Serial.println ("Initializing...");

  // Wifi Connection
  sample_init(ssid, pass);

  // Initialize the TF Mini sensor
      // Initialize the data rate for the SoftwareSerial port
  mySerial.begin(TFMINI_BAUDRATE);
  tfmini.begin(&mySerial);   

  // Set the Red Led to Blink to get porgram status
    // Blink Red slow : no wifi
    // Blink Red quicker : wifi
  pinMode(0, OUTPUT);
  pinMode(2, OUTPUT);

  Serial.println ("End Initialization");
  digitalWrite(0, HIGH);   // turn the LED on (HIGH is the voltage level) 
  digitalWrite(2, HIGH);   // turn the LED on (HIGH is the voltage level) 
}

// Card Loop, should be done in Azure Monitor 
static bool done = false;
void loop() {

  if (!done)
  {
      // Run the sample
      // You must set the device id, device key, IoT Hub name and IotHub suffix in
      // azure_monitoring.h
      azure_monitoring_run();
      done = true;
  }
  else
  {
    delay(1000);
  }
}
