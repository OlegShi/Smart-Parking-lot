<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RequestSql.aspx.cs" Inherits="parking.RequestSql" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
        <h1>SQL Tools</h1>
        <p> Enter SQL Commands to manage the Parking Database.</p>
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
                    <asp:Button ID="butSelect" runat="server" CssClass="fRight btn btn-primary" OnClick="butSelect_Click" Text="Proceed" />
                    <asp:TextBox ID="rtbRequest"  CssClass="form-control" runat="server"></asp:TextBox>
                    <div class="tablecontent">
                    <asp:Label ID="labelParkingGrid" runat="server"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
