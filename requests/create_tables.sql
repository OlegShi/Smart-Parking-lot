-- Empty table
TRUNCATE TABLE camera_pictures;

-- Destroy table
DROP TABLE camera_pictures;

-- Create Table camera pictures
CREATE TABLE 	
(
    id int IDENTITY(1,1) PRIMARY KEY,
    hardOcr varchar(20),
    cleanOcr varchar(20),
    ident varchar(20),
    enter varchar(1),
    photoname varchar(50),
    dt DATETIME
);

-- Empty table
TRUNCATE TABLE cars;

-- Destroy table
DROP TABLE cars;

-- Create Table cars
CREATE TABLE cars
(
    id int IDENTITY(1,1) PRIMARY KEY,
    number varchar(20),
    enter varchar(1),

    enter_dt DATETIME,
    id_enter_gate int,
    id_enter_picture int,

    exit_dt DATETIME,
    id_exit_gate int,
    id_exit_picture int,

    payment_mns int,
    payment_dhm varchar(20),
    payment_dt DATETIME,
    id_payment int,
    paid int,
    amount int,
    total_mns int,
    total_dhm varchar(20),

    dt DATETIME,

);

-- Empty table
TRUNCATE TABLE parking;

-- Destroy table
DROP TABLE parking;

-- Create Table cars
CREATE TABLE parking
(
    id int IDENTITY(1,1) PRIMARY KEY,
    level varchar(4),
    row varchar(4),
    number int,
    busy int,
    dt DATETIME
);

-- Empty table
TRUNCATE TABLE sensors;

-- Destroy table
DROP TABLE sensors;

-- Create Table cars
CREATE TABLE sensors
(
    id int IDENTITY(1,1) PRIMARY KEY,
    ident varchar(20),
    id_parking int,
    dt DATETIME
);

-- Empty table
TRUNCATE TABLE cameras;

-- Destroy table
DROP TABLE cameras;

-- Create Table cars
CREATE TABLE cameras
(
    id int IDENTITY(1,1) PRIMARY KEY,
    ident varchar(20),
    id_parking int,
    enter varchar(1),
    dt DATETIME
);

-- Data for tests
insert into parking ( level, row, number, busy) VALUES ('0', 'GATE', '1', '0');
insert into parking ( level, row, number, busy) VALUES ('0', 'GATE', '2', '0');
insert into parking ( level, row, number, busy) VALUES ('0', 'A', '1', '0');
insert into parking ( level, row, number, busy) VALUES ('0', 'A', '2', '0');
insert into parking ( level, row, number, busy) VALUES ('0', 'A', '3', '0');
insert into parking ( level, row, number, busy) VALUES ('0', 'A', '4', '0');
insert into parking ( level, row, number, busy) VALUES ('0', 'A', '5', '0');
insert into parking ( level, row, number, busy) VALUES ('0', 'B', '1', '0');
insert into parking ( level, row, number, busy) VALUES ('0', 'B', '2', '0');
insert into parking ( level, row, number, busy) VALUES ('0', 'B', '3', '0');

insert into sensors (ident, id_parking) VALUES ('SENSOR1', 3);
insert into sensors (ident, id_parking) VALUES ('SENSOR2', 4);
insert into sensors (ident, id_parking) VALUES ('SENSOR3', 5);
insert into sensors (ident, id_parking) VALUES ('SENSOR4', 6);
insert into sensors (ident, id_parking) VALUES ('SENSOR5', 7);
insert into sensors (ident, id_parking) VALUES ('SENSOR6', 8);
insert into sensors (ident, id_parking) VALUES ('SENSOR7', 9);
insert into sensors (ident, id_parking) VALUES ('SENSOR8', 10);

insert into cameras (ident, id_parking, enter) VALUES ('CAMERA1', 1, '1');
insert into cameras (ident, id_parking, enter) VALUES ('CAMERA2', 2, '0');

-- Connection strings etc for tests and management
-- Empty table
TRUNCATE TABLE azure;

-- Destroy table
DROP TABLE azure;

CREATE TABLE azure
(
    id int IDENTITY(1,1) PRIMARY KEY,
    type varchar(20),
    ident varchar(20),
    ConnectionString varchar(250),
    EventsEndpoint varchar(250),
    EventsPath varchar(50),
    PrimaryKey varchar(100),
    dt DATETIME
);

-- Parameters for Sensors IOTHUB
insert into azure (type, ident, ConnectionString, EventsEndpoint, EventsPath, PrimaryKey, dt) VALUES ('IOTHUB', 'SENSOR', 'HostName=CharleneIoTHub.azure-devices.net;DeviceId=MySensorDevice;SharedAccessKey=cQikpyDLBxaP39faFhwnmmmokdsPhUlpMhoP4XcuneM=', 'sb://iothub-ns-charleneio-523617-3c83d6adb4.servicebus.windows.net/', 'charleneiothub', '4y4eTaGS7Nq4BfARfab4iN375BWAi/JUi/EgvtnbrPo=', '2018-08-12 10:00');
insert into azure (type, ident, ConnectionString, EventsEndpoint, EventsPath, PrimaryKey, dt) VALUES ('STORAGE', 'SENSOR', 'DefaultEndpointsProtocol=https;AccountName=charlenestorage;AccountKey=dQsd0EFsRmb4JKpvXV4iXsQkVjDH3ioAChn/bxZbO6oTzExmjyjQdtX2HBWHiunNl208Ks0TjhrPIswJt8y0Yg==;EndpointSuffix=core.windows.net', 'devicetfminicontainer', '', '', '2018-08-12 10:00');
-- Parameters for Camera 
insert into azure (type, ident, ConnectionString, EventsEndpoint, EventsPath, PrimaryKey, dt) VALUES ('STORAGE', 'CAMERA', 'DefaultEndpointsProtocol=https;AccountName=charlenestorage;AccountKey=dQsd0EFsRmb4JKpvXV4iXsQkVjDH3ioAChn/bxZbO6oTzExmjyjQdtX2HBWHiunNl208Ks0TjhrPIswJt8y0Yg==;EndpointSuffix=core.windows.net', 'devicecameracontainer', '', '', '2018-08-12 10:00');
-- Parameters for OCR Computer Vision
insert into azure (type, ident, ConnectionString, EventsEndpoint, EventsPath, PrimaryKey, dt) VALUES ('VISION', 'OCR', '','','','14c5d8b72bd44efc8ad67a22f84232e5', '2018-08-12 10:00');


-- Empty table
TRUNCATE TABLE subscription;

-- Destroy table
DROP TABLE subscription;

-- Create Table cars
CREATE TABLE subscription
(
    id int IDENTITY(1,1) PRIMARY KEY,
    ident varchar(20),
    from_dt DATE,
    to_dt DATE,
    id_rate int,
    dt DATETIME
);

-- Empty table
TRUNCATE TABLE rates;

-- Destroy table
DROP TABLE rates;

-- Create Table cars
CREATE TABLE rates
(
    id int IDENTITY(1,1) PRIMARY KEY,
    ratename varchar(20),
    unit int,
    dt DATETIME
);

-- Empty table
TRUNCATE TABLE rate_details;

-- Destroy table
DROP TABLE rate_details;

-- Create Table cars
CREATE TABLE rate_details
(
    id int IDENTITY(1,1) PRIMARY KEY,
    id_rate int,
    limitmns int,
    price int,
    dt DATETIME
);

insert into rates (ratename, unit, dt) VALUES ('RATE REGULAR', '30', '2018-08-16 08:10');
insert into rates (ratename, unit, dt) VALUES ('RATE FREE', '30', '2018-08-16 08:12');

insert into rate_details (id_rate, limitmns, price, dt) VALUES ('1', '30', '20', '2018-08-16 08:15');
insert into rate_details (id_rate, limitmns, price, dt) VALUES ('1', '120', '15', '2018-08-16 08:15');
insert into rate_details (id_rate, limitmns, price, dt) VALUES ('1', '480', '10', '2018-08-16 08:15');
insert into rate_details (id_rate, limitmns, price, dt) VALUES ('1', '600000', '8', '2018-08-16 08:15');
insert into rate_details (id_rate, limitmns, price, dt) VALUES ('2', '600000', '0', '2018-08-16 08:15');

insert into subscription (ident, from_dt, to_dt, id_rate, dt) VALUES ('24419901', '2018-08-15','2018-09-30','2','2018-08-16 08:15');
insert into subscription (ident, from_dt, id_rate, dt) VALUES ('', '2018-08-15','1','2018-08-16 08:15');



