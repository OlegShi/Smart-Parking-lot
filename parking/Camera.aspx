<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Camera.aspx.cs" Inherits="parking.WebForm1" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
        <h1>CAMERA PICTURES</h1>
        <p>
            Pictures taken by the cameras on Gates.</p>
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
            <div class="form-row align-items-center">
                <div class="col-md-2 fLeft mb10">
                    <asp:Label ID="labelCarsDateFilter" runat="server" Text="Date"></asp:Label>
                    <asp:TextBox ID="calDateText" runat="server" CssClass="posrel fdate form-control"></asp:TextBox>
                </div>
                <div class="col-md-2 fLeft mb10">
                    <asp:Label ID="LabelCar1" runat="server" CssClass="col-form-label" Text="Car Number (full or partial)"></asp:Label>
                    <asp:TextBox ID="dbCarsFilter" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="menuAllCars" runat="server" CssClass="fRight btn btn-primary" OnClick="menuAllCars_Click" Text="Search" />
                    <div class="tablecontent">
                    <asp:Label ID="labelCarGrid" runat="server"></asp:Label>
                    </div>
               </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
