<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="EasyPersistDemoSite._Default" Title="EasyPersist Web Demo - City List" MasterPageFile="~/Public.master" %>

<asp:Content ContentPlaceHolderID="cphm" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server" />
	<h1>How to load a list of objects from db and show them on the web page.</h1>
	<div>This page allows you to view the list of cities in the system. 
	<br/>You can create a new city or edit a city.
	<br/>This is just a demo page so it is simplified as much as possible</div>
	<br />
	<br />
	<table class="listtable" cellpadding="2" cellspacing="1">
		<tr style="font-weight: bold;" class="listheader">
			<td class="listheader">Name</td>
			<td class="listheader">County</td>
			<td class="listheader">State</td>
			<td class="listheader">Actions (<a href="EditCity.aspx">Add City</a>)</td>
		</tr>
		<asp:Repeater ID="CitiesRepeater" runat="server">
			<ItemTemplate>
				<tr class="content">
					<td class="content"><%#Eval("Name")%></td>
					<td class="content"><%#Eval("County.Name")%></td>
					<td class="content"><%#Eval("County.State.Name")%></td>
					<td class="content"><a href="EditCity.aspx?cityid=<%#Eval("Id")%>">Edit</a></td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>	
</asp:Content>
