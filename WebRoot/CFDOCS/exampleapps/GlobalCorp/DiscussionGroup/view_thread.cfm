<!--- ColdFusion(r) Express Global Corp. Example Application --->


<!--- 	Set our default form values. In form.message, we set the default equal to the client.sig value. since we want some white space between the user's message and the sig, we add it by using the Chr(10) value. Also, we put in the HTML <BR> tag in order to format the signature --->
<CFPARAM NAME="Form.Subject" DEFAULT="">
<CFPARAM NAME="Form.Message" DEFAULT="#Chr(10)##Chr(10)##Replace(Client.Sig,Chr(10),"<BR>#Chr(10)#")#">
<CFPARAM NAME="ERROR" DEFAULT="">

<!--- 	Verify good id first.  --->
<CFIF NOT IsDefined("URL.id")>
	<CFLOCATION URL="index.cfm" ADDTOKEN="No">
<CFELSE>
	<CFQUERY NAME="ThreadInfo" DATASOURCE="#application.ds#">
		SELECT 	strThrd 
		FROM 	tblThrd
		WHERE  	strThrdID = '#URL.id#'
	</CFQUERY>
	<CFIF ThreadInfo.RecordCount IS 0>
		<CFLOCATION URL="index.cfm" ADDTOKEN="No">
	<CFELSEIF NOT Len(Trim(Form.Subject))>
		<CFSET Form.Subject="RE: " & ThreadInfo.strThrd>
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
			(
			strMessID,
			strThdIDFK,
			strEmpIDFK,
			strSubject,
			dtPosted,
			glbMessage
			)
			VALUES
			(
			'#CreateUUID()#',
			'#URL.id#',
			'#Client.Employee_ID#',
			'#HTMLEditFormat(Form.Subject)#',
			#CreateODBCDateTime(now())#,
			'#Form.Message#'
			)
		</CFQUERY>
		<CFSET Form.Message="#Chr(10)##Chr(10)##Client.Sig#">
	</CFIF>
</CFIF>	

<CFQUERY NAME="GetMessages" DATASOURCE="#application.DS#">
	SELECT 		strSubject,
				dtPosted,
				glbMessage,
				strFname,
				strLname,
				strEmail
	FROM 		tblMes,
				tblEmp
	WHERE  		strThdIDFK = '#URL.id#'
	AND    		strEmpIDFK = strEmpID
	ORDER BY 	dtPosted <CFIF Client.PostOrder IS "Newest">DESC</CFIF>
</CFQUERY>


<!--- Include the Global Header --->
<CFSET header.page_name=ThreadInfo.strThrd>
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





<P>
<B>Messages posted to thread:</B>
<TABLE WIDTH="100%">


<CFOUTPUT QUERY="GetMessages">
	<TR>
		<TD><A HREF="##Mess#GetMessages.CurrentRow#">#strSubject#</A></TD>
		<TD>#DateFormat(dtPosted,"m/d/yyyy")# #TimeFormat(dtPosted,"h:mm tt")#</TD>
	</TR>
</CFOUTPUT>

	<TR>
		<TD COLSPAN="2">
			<BR>
			[ <A HREF="#PostToThread">Post to Thread</A> ]
		</TD>
	</TR>	

</TABLE>

<P>
<HR NOSHADE SIZE="1">


<CFOUTPUT QUERY="GetMessages">
	<A NAME="Mess#GetMessages.CurrentRow#"></A>
	<CFINCLUDE TEMPLATE="_message_display.cfm">
</CFOUTPUT>

<CFOUTPUT>

<A NAME="PostToThread"></A>
<TABLE>
<FORM ACTION="view_thread.cfm?id=#URL.id#" METHOD="POST">
<TR>
	<TD COLSPAN="2">
	<B>Post to Thread</B>
	</TD>
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