<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="ToolsApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

   

    <div class="row">
        <div class="col-md-4">
            <h2>Fetching Calander Events</h2>
             <asp:Button runat="server" ID="getCalander" OnClick="getCalander_Click" Text="Create Event" />
            <asp:Button runat="server" ID="DeleteEvent" OnClick="DeleteEvent_Click" Text="Delete Event" />
        </div>
     <div id="calander" runat="server" visible="">
         <iframe src="https://calendar.google.com/calendar/embed?src=sohailahmedhanfi%40gmail.com&ctz=Asia%2FKarachi" style="border: 0" width="800" height="600" frameborder="0" scrolling="no"></iframe>
     </div>
    </div>

</asp:Content>
