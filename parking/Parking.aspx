<%@ Page Title="Parking" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Parking.aspx.cs" Inherits="parking.About" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>PARKING</h1>
        <p>
            Visualize the parked car in the parking.</p>
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
                    <asp:Label ID="Label1" runat="server" CssClass="col-form-label" Text="Parking Level"></asp:Label>
                    <asp:DropDownList ID="ddParkingLevel"  CssClass="form-control" runat="server"></asp:DropDownList>
                    <asp:Button ID="butParking" runat="server" CssClass="fRight btn btn-primary" OnClick="butParking_Click" Text="Refresh" />
                    <asp:CheckBox ID="cbParkingAuto" runat="server" Checked="True" />
                    <asp:Button ID="butClearParking" runat="server" CssClass="fRight btn btn-danger" OnClick="butClearParking_Click" Text="Clear" />
                    <asp:Label ID="LabelRefrehParking" runat="server" Text="Auto Refresh"></asp:Label>
                    <div class="tablecontent">
                    <asp:Label ID="labelParkingGrid" runat="server"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
