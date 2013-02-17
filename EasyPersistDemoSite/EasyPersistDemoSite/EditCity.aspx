<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditCity.aspx.cs" Inherits="EditCity" MasterPageFile="~/Public.master" Title="EasyPersist Web Demo - Edit City " %>

<%@ Register Assembly="ConvincingMail.AdvancedAutoSuggest" Namespace="ConvincingMail.AdvancedAutoSuggest" TagPrefix="cc1" %>

<asp:Content ContentPlaceHolderID="cphm" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" />
<h1>How to edit an object and save results in db.</h1>
	<div>This page allows you add or edit an object (City)
	<br/>Depending on querystring parameters this page will show you the city info to edit or empty boxes to create a new city.
	<br/>This page uses <a href="http://convincingmail.com/autosuggest-autocomplete.aspx">ConvincingMail AJAX AutoSuggest control</a> to show county suggestions.	
	<br/>This is just a demo page so it is simplified as much as possible</div>
	<br />
	<br />
	<table class="edittable" style="" cellspacing="1" cellpadding="2">
		<tr class="editheader"><td colspan="2" class="editheader">Edit City</td></tr>
		<tr class="editcontent">
			<td class="editlabel">County <span class="rc">*</span></td>
			<td class="editvalue">				
				<asp:TextBox Text="<%#City.County.Id%>" ID="CountyIdHidden" style="display:none;" runat="server" />
				<asp:TextBox ID="CountyTextBox" Text="<%#Server.HtmlDecode(City.County.Name)%>" runat="server"></asp:TextBox>
				<br /><span class="editcomment">Start typing in the box to see a list of suggestions. You should select one from the list.</span>
				<asp:CustomValidator ID="CountyIdCustomValidator" ValidateEmptyText="true" ControlToValidate="CountyIdHidden" OnServerValidate="CountyIdCustomValidator_OnServerValidate" runat="server" ErrorMessage="Please select county from the list" Display="Dynamic"><br />Please select county from the suggestions list.</asp:CustomValidator></td>
		</tr>
		<tr class="editcontent">
			<td class="editlabel">City Name <span class="rc">*</span></td>
			<td class="editvalue">
				<asp:TextBox ID="NameTextBox" Text="<%#Server.HtmlDecode(City.Name)%>"  runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="NameTextBox" Display="Dynamic" ErrorMessage="RequiredFieldValidator"><br />Name is required for the city.</asp:RequiredFieldValidator></td>
		</tr>
		<tr class="editcontent">
			<td class="editlabel">Check to make active</td>
			<td class="editvalue">
				<asp:CheckBox ID="IsActiveCheckBox" Checked="<%#City.IsActive%>" runat="server" /></td>
		</tr>
		<tr class="editcontent">
			<td class="editlabel">Settlement Type <span class="rc">*</span></td>
			<td class="editvalue">
				<asp:DropDownList ID="SettlementTypeDropDownList" EnableViewState="false" runat="server">					
				</asp:DropDownList></td>
		</tr>	
		<tr class="editcontent"><td colspan="2" style="text-align:center;" class="editvalue">
			<asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_OnClick" /></td></tr>
	</table>
</asp:Content>
