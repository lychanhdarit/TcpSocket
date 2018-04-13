﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"BaseMessage")]
    public partial class BaseMessage : global::ProtoBuf.IExtensible
    {
        public BaseMessage() { }

        private RabbitMQ.BaseMessage.MsgType _msgType;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"msgType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public RabbitMQ.BaseMessage.MsgType msgType
        {
            get { return _msgType; }
            set { _msgType = value; }
        }
        private string _providerId = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"providerId", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string providerId
        {
            get { return _providerId; }
            set { _providerId = value; }
        }
        private string _senderId = "";
        [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"senderId", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string senderId
        {
            get { return _senderId; }
            set { _senderId = value; }
        }
        private RabbitMQ.RegVehicle _msg;
        [global::ProtoBuf.ProtoMember(400, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public RabbitMQ.RegVehicle msg
        {
            get { return _msg; }
            set { _msg = value; }
        }
        private RabbitMQ.RegDriver _msgRegDriver;
        [global::ProtoBuf.ProtoMember(410, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public RabbitMQ.RegDriver msgRegDriver
        {
            get { return _msgRegDriver; }
            set { _msgRegDriver = value; }
        }
        private RabbitMQ.RegCompany _msgRegCompany;
        [global::ProtoBuf.ProtoMember(420, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public RabbitMQ.RegCompany msgRegCompany
        {
            get { return _msgRegCompany; }
            set { _msgRegCompany = value; }
        }
        private RabbitMQ.WayPoint _msgWayPoint;
        [global::ProtoBuf.ProtoMember(100, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public RabbitMQ.WayPoint msgWayPoint
        {
            get { return _msgWayPoint; }
            set { _msgWayPoint = value; }
        }
        [global::ProtoBuf.ProtoContract(Name = @"MsgType")]
        public enum MsgType
        {

            [global::ProtoBuf.ProtoEnum(Name = @"WayPoint", Value = 100)]
            WayPoint = 100,

            [global::ProtoBuf.ProtoEnum(Name = @"RegVehicle", Value = 400)]
            RegVehicle = 400,

            [global::ProtoBuf.ProtoEnum(Name = @"RegDriver", Value = 410)]
            RegDriver = 410,

            [global::ProtoBuf.ProtoEnum(Name = @"RegCompany", Value = 420)]
            RegCompany = 420
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"RegVehicle")]
    public partial class RegVehicle : global::ProtoBuf.IExtensible
    {
        public RegVehicle() { }

        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }
        private RabbitMQ.RegVehicle.VehicleType _vehicleType;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"vehicleType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public RabbitMQ.RegVehicle.VehicleType vehicleType
        {
            get { return _vehicleType; }
            set { _vehicleType = value; }
        }
        private string _driver;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"driver", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string driver
        {
            get { return _driver; }
            set { _driver = value; }
        }
        private string _company;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"company", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string company
        {
            get { return _company; }
            set { _company = value; }
        }
        private int _deviceModelNo = default(int);
        [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name = @"deviceModelNo", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        [global::System.ComponentModel.DefaultValue(default(int))]
        public int deviceModelNo
        {
            get { return _deviceModelNo; }
            set { _deviceModelNo = value; }
        }
        private string _deviceModel = "";
        [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name = @"deviceModel", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string deviceModel
        {
            get { return _deviceModel; }
            set { _deviceModel = value; }
        }
        private string _deviceId = "";
        [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"deviceId", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string deviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }
        private string _sim = "";
        [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name = @"sim", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string sim
        {
            get { return _sim; }
            set { _sim = value; }
        }
        private int _datetime = default(int);
        [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name = @"datetime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        [global::System.ComponentModel.DefaultValue(default(int))]
        public int datetime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }
        private string _vin = "";
        [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name = @"vin", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string vin
        {
            get { return _vin; }
            set { _vin = value; }
        }
        private float _capacity = default(float);
        [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name = @"capacity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }
        [global::ProtoBuf.ProtoContract(Name = @"VehicleType")]
        public enum VehicleType
        {

            [global::ProtoBuf.ProtoEnum(Name = @"Khach", Value = 100)]
            Khach = 100,

            [global::ProtoBuf.ProtoEnum(Name = @"Bus", Value = 200)]
            Bus = 200,

            [global::ProtoBuf.ProtoEnum(Name = @"HopDong", Value = 300)]
            HopDong = 300,

            [global::ProtoBuf.ProtoEnum(Name = @"DuLich", Value = 400)]
            DuLich = 400,

            [global::ProtoBuf.ProtoEnum(Name = @"Container", Value = 500)]
            Container = 500,

            [global::ProtoBuf.ProtoEnum(Name = @"XeTai", Value = 600)]
            XeTai = 600,

            [global::ProtoBuf.ProtoEnum(Name = @"Taxi", Value = 700)]
            Taxi = 700,

            [global::ProtoBuf.ProtoEnum(Name = @"TaxiTai", Value = 800)]
            TaxiTai = 800,

            [global::ProtoBuf.ProtoEnum(Name = @"XeDauKeo", Value = 900)]
            XeDauKeo = 900
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"RegDriver")]
    public partial class RegDriver : global::ProtoBuf.IExtensible
    {
        public RegDriver() { }

        private string _driver;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"driver", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string driver
        {
            get { return _driver; }
            set { _driver = value; }
        }
        private string _name = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        private int _datetimeIssue = default(int);
        [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"datetimeIssue", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        [global::System.ComponentModel.DefaultValue(default(int))]
        public int datetimeIssue
        {
            get { return _datetimeIssue; }
            set { _datetimeIssue = value; }
        }
        private int _datetimeExpire = default(int);
        [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name = @"datetimeExpire", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        [global::System.ComponentModel.DefaultValue(default(int))]
        public int datetimeExpire
        {
            get { return _datetimeExpire; }
            set { _datetimeExpire = value; }
        }
        private string _regPlace = "";
        [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name = @"regPlace", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string regPlace
        {
            get { return _regPlace; }
            set { _regPlace = value; }
        }
        private string _license = "";
        [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name = @"license", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string license
        {
            get { return _license; }
            set { _license = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"RegCompany")]
    public partial class RegCompany : global::ProtoBuf.IExtensible
    {
        public RegCompany() { }

        private string _company;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"company", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string company
        {
            get { return _company; }
            set { _company = value; }
        }
        private string _name = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _address = "";
        [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"address", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string address
        {
            get { return _address; }
            set { _address = value; }
        }
        private string _tel = "";
        [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name = @"tel", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string tel
        {
            get { return _tel; }
            set { _tel = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"WayPoint")]
    public partial class WayPoint : global::ProtoBuf.IExtensible
    {
        public WayPoint() { }

        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }
        private string _driver = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"driver", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string driver
        {
            get { return _driver; }
            set { _driver = value; }
        }
        private float _speed;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"speed", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        public float speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        private int _datetime;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"datetime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int datetime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }
        private double _x;
        [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name = @"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double x
        {
            get { return _x; }
            set { _x = value; }
        }
        private double _y;
        [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name = @"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double y
        {
            get { return _y; }
            set { _y = value; }
        }
        private float _z = default(float);
        [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"z", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float z
        {
            get { return _z; }
            set { _z = value; }
        }
        private float _heading = default(float);
        [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name = @"heading", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float heading
        {
            get { return _heading; }
            set { _heading = value; }
        }
        private bool _ignition = default(bool);
        [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name = @"ignition", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool ignition
        {
            get { return _ignition; }
            set { _ignition = value; }
        }
        private bool _door = default(bool);
        [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name = @"door", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool door
        {
            get { return _door; }
            set { _door = value; }
        }
        private bool _aircon = default(bool);
        [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name = @"aircon", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool aircon
        {
            get { return _aircon; }
            set { _aircon = value; }
        }

        //For save data to raw table
        private double _maxValidSpeed = default(double);
        [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name = @"maxvalidspeed", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(double))]
        public double maxvalidspeed
        {
            get { return _maxValidSpeed; }
            set { _maxValidSpeed = value; }
        }

        private float _vss = default(float);
        [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name = @"vss", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float vss
        {
            get { return _vss; }
            set { _vss = value; }
        }

        private string _location = default(string);
        [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name = @"location", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(string))]
        public string location
        {
            get { return _location; }
            set { _location = value; }
        }
        //End save data to raw table

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }
}
