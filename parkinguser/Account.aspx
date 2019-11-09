<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="parking.Account" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Your Account</h1>
        <p>
            Visualize your current amount and your history.
        </p>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="butParking" runat="server" CssClass="fRight btn btn-primary" OnClick="butParking_Click" Text="Refresh" />
                    <div class="col-md-2 fLeft mb10 filtercontent">
                        <asp:Label ID="LabelCar1" runat="server" CssClass="col-form-label" Text="Car Number"></asp:Label>
                        <asp:TextBox ID="dbCarsFilter" runat="server" CssClass="form-control" ReadOnly="True"></asp:TextBox>
                    </div>                    
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
