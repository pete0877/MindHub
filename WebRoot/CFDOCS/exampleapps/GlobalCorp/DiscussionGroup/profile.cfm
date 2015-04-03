<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- The options set for the user on this page are stored in CLIENT variables persistent on the server --->

<!--- Update settings if the form has been submitted--->
<CFIF IsDefined("Form.gogo")>
	<CFSET Client.PostOrder=Form.Order>
	<CFSET Client.Sig=Form.Sig>
	<CFSET Message="Your profile has been updated.">
</CFIF>

<!--- Include the Global Header --->
<CFSET header.page_name="Your Profile Settings">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">


<!--- Discussion Group Menu --->
<FONT SIZE="2">
{
<A HREF="index.cfm">Thread List</A>
 | 
<A HREF="new_thread.cfm">Create New Thread</A>
 | 
<A HREF="search.cfm">Search Messages</A>
}
</FONT>



<CFOUTPUT>

<P>
<CFIF IsDefined("Message")>
	<B>#Message#</B>
	<P>
</CFIF>
<B>Please update your settings below.</B>

<P>
<!--- Form to update settings --->
<FORM ACTION="profile.cfm" METHOD="POST">
Display order for messages:
<BR>
	<SELECT NAME="Order">
	<OPTION VALUE="Oldest"<CFIF CLIENT.POSTORDER IS "Oldest">SELECTED</CFIF>>Oldest First
	<OPTION VALUE="Newest"<CFIF CLIENT.POSTORDER IS "Newest">SELECTED</CFIF>>Newest First
	</SELECT>
<P>

Your message signature:
<BR>
<TEXTAREA NAME="sig" COLS="30" ROWS="5">#Client.Sig#</TEXTAREA>

<P>	
<INPUT TYPE="submit" NAME="gogo" VALUE="Update Profile">
</FORM>
</CFOUTPUT>


<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">
