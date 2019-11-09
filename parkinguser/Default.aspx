<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="parking._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

      <div class="jumbotron">
        <h1>Free Places</h1>
        <p>
            Please have a look on remaining free places and find your place more easily.
        </p>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="butParking" runat="server" CssClass="fRight btn btn-primary" OnClick="butParking_Click" Text="Refresh" />                   
                    <asp:CheckBox ID="cbParkingAuto" runat="server" Checked="True" />
                    <asp:Label ID="LabelRefrehParking" runat="server" Text="Auto Refresh"></asp:Label>
                    <div class="tablecontent">
                    <asp:Label ID="labelParkingGrid" runat="server"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
