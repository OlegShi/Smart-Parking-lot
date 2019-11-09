<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Rates.aspx.cs" Inherits="parking.Rates" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
          <div class="jumbotron">
        <h1>Rates</h1>
        <p>
            Please have a look on Rates for your car number.
        </p>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="tablecontent">
                    <asp:Label ID="labelParkingGrid" runat="server"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
