// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#ifndef IOT_CONFIGS_H
#define IOT_CONFIGS_H

/**
 * Find under Microsoft Azure IoT Suite -> DEVICES -> <your device> -> Device Details and Authentication Keys
 * String containing Hostname, Device Id & Device Key in the format:
 *  "HostName=<host_name>;DeviceId=<device_id>;SharedAccessKey=<device_key>"    
 */
#define IOT_CONFIG_CONNECTION_STRING    "HostName=CharleneIoTHub.azure-devices.net;DeviceId=MySensorDevice;SharedAccessKey=cQikpyDLBxaP39faFhwnmmmokdsPhUlpMhoP4XcuneM="
// Identification for the Parking : Name of the SENSOR : SENSOR1...SENSOR8 what we have in the database, # should be replace by 0 or 1 before sending
#define IOT_DEVICE_PRESENCE_JSON_STRING "{\"type\":\"sensor\",\"ident\":\"SENSOR2\",\"enter\":\"#\"}";

// LIMITS and MAX RETRY Before to consider stable
#define SENSOR_MAX_RETRY_BEFORE_STABLE_VALUE  5
// Between SENSOR_MIN_LIMIT_VALUE and SENSOR_MAX_LIMIT_VALUE we consider Near, Car is present
// Under SENSOR_MIN_LIMIT_VALUE : no valid value
// Up to SENSOR_MAX_LIMIT_VALUE : Far Value, No car
#define SENSOR_MIN_LIMIT_VALUE                40
#define SENSOR_MAX_LIMIT_VALUE                100

/** 
 * Choose the transport protocol
 */
#define IOT_CONFIG_MQTT                 // uncomment this line for MQTT
// #define IOT_CONFIG_HTTP              // uncomment this line for HTTP

#endif /* IOT_CONFIGS_H */
