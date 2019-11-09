<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="parking.Login" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>User Login</h1>
        <p>
            Enter your Car Number and Visualize your current amount, your history and proceed to payment.
        </p>
    </div>
    
    <div class="container login-content">
        <div class="card card-container">
            <img id="profile-img" class="profile-img-card" src="images/car.jpg" />
            <p id="profile-name" class="profile-name-card"></p>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label ID="LabelCar1" runat="server" CssClass="col-form-label" Text="Car Number (ex:7955059,24419901)"></asp:Label>
                    <asp:TextBox ID="dbCarsFilter" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:Button ID="butLogin" runat="server" CssClass="btn btn-primary" OnClick="butLogin_Click" Text="Login" />
                    <div class="tablecontent">
                    <asp:Label ID="labelLoginGrid" runat="server"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
