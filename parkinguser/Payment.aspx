<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="parking.Payment" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Payment</h1>
        <p>
            Proceed to your payment
        </p>
    </div>

    <div class="container login-content">
        <div class="card card-container">
            <img id="profile-img" class="profile-img-card" src="images/card.png" />
            <p id="profile-name" class="profile-name-card"></p>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                     <div class="tablecontent">
                    <asp:Label ID="labelPaymentGrid" runat="server"></asp:Label>
                    </div>                   
                    <asp:TextBox ID="ccName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                    <asp:TextBox ID="ccNumber" runat="server" CssClass="form-control" placeholder="Number"></asp:TextBox>
                    <asp:TextBox ID="ccDate" runat="server" CssClass="form-control" placeholder="MM/YY"></asp:TextBox>
                    <asp:TextBox ID="ccCvv" runat="server" CssClass="form-control" placeholder="CVV"></asp:TextBox>
                    <asp:Button ID="butCancel" runat="server" CssClass="btn btn-alert" OnClick="butCancel_Click" Text="Cancel" />                   
                    <asp:Button ID="butPay" runat="server" CssClass="btn btn-primary" OnClick="butPay_Click" Text="Payment" />                   

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
