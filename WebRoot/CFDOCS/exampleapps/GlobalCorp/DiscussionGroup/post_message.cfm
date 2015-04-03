<!--- ColdFusion(r) Express Global Corp. Example Application --->


<!--- Set default form values and an error variable. --->
<CFPARAM NAME="Form.Subject" DEFAULT="">
<CFPARAM NAME="Form.Message" DEFAULT="#Chr(10)##Chr(10)##Replace(Client.Sig,Chr(10),"<BR>#Chr(10)#")#">
<CFPARAM NAME="ERROR" DEFAULT="">

<!--- 	Verify good id first.  --->
<CFIF NOT IsDefined("URL.id")>
	<CFLOCATION URL="index.cfm" ADDTOKEN="No">
<CFELSE>
	<CFQUERY NAME="ThreadInfo" DATASOURCE="#application.ds#">
		SELECT strThrd FROM tblThrd
		WHERE  strThrdID = '#URL.id#'
	</CFQUERY>
	<!--- Again, if something goes wrong, we move em to the home page. like i said above, this is cool if  someone creates a link to your site and doesn't do it correctly. --->
	<CFIF ThreadInfo.RecordCount IS 0>
		<CFLOCATION URL="index.cfm" ADDTOKEN="No">
	</CFIF>
</CFIF>

<!--- Validate form input --->
<CFIF IsDefined("Form.gogo")>
	<CFIF NOT Len(Trim(Form.Subject))>
		<CFSET ERROR=ERROR & "You must enter a subject.<BR>">
	</CFIF>
	<CFIF NOT Len(Trim(Form.Message))>
		<CFSET ERROR=ERROR & "You must enter a message.<BR>">
	</CFIF>
	<CFIF NOT Len(ERROR)>
		<!--- A good post, let's add it to the db --->
		<CFQUERY NAME="PostMessage" DATASOURCE="#application.ds#">
			INSERT INTO tblMes
			(strMessID,
			strThdIDFK,
			strEmpIDFK,
			strSubject,
			dtPosted,
			glbMessage)
			VALUES
			('#CreateUUID()#',
			'#URL.id#',
			'#Client.Employee_ID#',
			'#HTMLEditFormat(Form.Subject)#',
			#CreateODBCDateTime(Now())#,
			'#Form.Message#')
		</CFQUERY>
		<CFLOCATION URL="view_thread.cfm?id=#URL.id#" ADDTOKEN="NO">
	</CFIF>
</CFIF>	


<!--- Include the Global Header --->
<CFSET header.page_name="Post Message">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<!--- Discussion Group Menu --->
<FONT SIZE="2">
{
<A HREF="index.cfm">Thread List</A>
 | 
<A HREF="new_thread.cfm">Create New Thread</A>
 |  
<A HREF="profile.cfm">Customize Profle</A>
 | 
<A HREF="search.cfm">Search Messages</A>
}
</FONT>



<!--- The Form for posting a message --->
<CFOUTPUT>
<FORM ACTION="post_message.cfm?id=#URL.id#" METHOD="POST">
<TABLE>
<!--- If there is an error message to show --->
<CFIF Len(ERROR)>
	<TR>
		<TD COLSPAN="2">Your form could not be submitted due to the following error(s):<BR>#ERROR#<BR></TD>
	</TR>
</CFIF>
<TR>
	<TD><B>Thread:</B></TD>
	<TD>#ThreadInfo.strThrd#</TD>
</TR>
<TR>
	<TD><B>Subject:</B></TD>
	<TD><INPUT TYPE="text" NAME="Subject" SIZE="35" VALUE="#HTMLEditFormat(Form.Subject)#"></TD>
</TR>
<TR>
	<TD COLSPAN="2"><B>Message:</B><BR>
	<TEXTAREA NAME="Message" COLS="40" ROWS="10" WRAP="virtual">#Form.Message#</TEXTAREA></TD>
</TR>
</TABLE>

<INPUT TYPE="submit" NAME="gogo" VALUE="Post Message">
</FORM>
</CFOUTPUT>

<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">