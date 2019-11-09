<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="parking.User"  Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Dear User</h1>
        <p>
            Enter your Car Number and Visualize your current amount, remaining free place and your history.
        </p>
    </div>


    <div class="row">
        <div class="col-md-12">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="butParking" runat="server" CssClass="fRight btn btn-primary" OnClick="butParking_Click" Text="Refresh" />
                    <div class="col-md-2 fLeft mb10 filtercontent">
                        <asp:Label ID="LabelCar1" runat="server" CssClass="col-form-label" Text="Car Number"></asp:Label>
                        <asp:TextBox ID="dbCarsFilter" runat="server" CssClass="form-control"></asp:TextBox>
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
