<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Simulation.aspx.cs" Inherits="parking.Simulation" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div class="jumbotron">
        <h1>Simulations</h1>
        <p>Simulate Sensor message and Image from Camera.</p>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Label ID="LabelUpd" runat="server" Text="Last Update"></asp:Label>
                <asp:Timer ID="TimerReadBlob" runat="server" Interval="10000" OnTick="TimerReadBlob_Tick">
                </asp:Timer>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <asp:Button ID="butSendSensor" runat="server" CssClass="fRight btn btn-primary" OnClick="butSendSensor_Click" Text="Sensor" />
                    <asp:CheckBox ID="cbCarParked" runat="server" Text="Car is parked" />
                    <asp:DropDownList ID="dbSensorList"  CssClass="form-control" runat="server"></asp:DropDownList>
                    <div class="tablecontent">
                    <asp:Label ID="labelSimuation" runat="server"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <hr />
    <div>
        <asp:FileUpload ID="FileUploadImg" CssClass="fRight btn btn-warning" runat="server" />
        <asp:Button ID="btnUpload" CssClass="fRight btn btn-warning" Text="Upload" runat="server" OnClick="UploadFile" />
        <asp:Image ID="Image2" runat="server" Width = "100" />
    </div>
</asp:Content>
