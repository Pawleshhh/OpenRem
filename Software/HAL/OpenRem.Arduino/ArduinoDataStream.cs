using OpenRem.HAL;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OpenRem.Arduino;

public class ArduinoDataStream : IDataStream
{
    private SerialPort serialPort;

    private readonly string comPort;
    private readonly ArduinoType arduinoType;

    public IObservable<byte> RawDataStream { get; private set; }

    public ArduinoDataStream(string comPort, ArduinoType arduinoType)
    {
        this.comPort = comPort;
        this.arduinoType = arduinoType;
    }

    public void Open()
    {
        this.serialPort = ArduinoSerialPortFactory.Create(this.comPort, this.arduinoType);
        this.serialPort.Open();
        RawDataStream = Observable.FromEventPattern<
            SerialDataReceivedEventHandler,
            SerialDataReceivedEventArgs>
        (
            handler => this.serialPort.DataReceived += handler,
            handler => this.serialPort.DataReceived -= handler
        ).SelectMany(_ =>
        {
            var buffer = new byte[1024];
            var ret = new List<byte>();
            var toRead = this.serialPort.BytesToRead;
            while (toRead > 0)
            {
                int bytesRead = this.serialPort.Read(buffer, 0, buffer.Length);
                ret.AddRange(buffer.Take(bytesRead));
                toRead -= bytesRead;
            }

            return ret;
        });
    }

    public void Close()
    {
        this.serialPort.Close();
    }

    public void Start()
    {
        SendStopMessage();
        this.serialPort.DiscardInBuffer();
        Task.Delay(200).ContinueWith((t) => { SendStartMessage(); });
    }

    public void Stop()
    {
        SendStopMessage();
        this.serialPort.DiscardInBuffer();
    }

    private void SendStartMessage()
    {
        var message = new byte[] { 1 };
        this.serialPort.Write(message, 0, message.Length);
    }

    private void SendStopMessage()
    {
        var message = new byte[] { 0 };
        this.serialPort.Write(message, 0, message.Length);
    }
}