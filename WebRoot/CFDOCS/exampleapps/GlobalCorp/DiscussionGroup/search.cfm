<!--- ColdFusion(r) Express Global Corp. Example Application --->


<CFPARAM NAME="Form.SearchTerm" DEFAULT="">

<CFIF Len(Trim(Form.SearchTerm))>
	<!--- Perform the search --->
	<CFQUERY NAME="SearchMessages" DATASOURCE="#application.ds#">
		SELECT		strThdIDFK,
					strEmpIDFK,
					strSubject,
					dtPosted,
					glbMessage,
					strFname,
					strLname,
					strEmail,
					strThrd
		FROM 		tblMes,
					tblEmp,
					tblThrd
		<CFIF Server.OS.Name CONTAINS "Windows">
		WHERE  		(glbMessage LIKE '%#Form.SearchTerm#%' 
		OR			strSubject LIKE '%#Form.SearchTerm#%')
		<CFELSE>
		WHERE       (Upper(glbMessage) LIKE '%#UCase(Form.SearchTerm)#%'
		OR          Upper(strSubject) LIKE '%#UCase(Form.SearchTerm)#%')
		</CFIF>
		AND    		strEmpIDFK = strEmpID
		AND    		strThdIDFK = strThrdID
		ORDER BY 	strThrd,
					dtposted <CFIF Client.PostOrder IS "Newest">DESC</CFIF>
	</CFQUERY>
</CFIF>

<!--- Include the Global Header --->
<CFSET header.page_name="Search Discussion Group">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<!--- Discussion Group Menu --->
<FONT SIZE="2">
{
<A HREF="index.cfm">Thread List</A>
 | 
<A HREF="new_thread.cfm">Create New Thread</A>
 | 
<A HREF="profile.cfm">Customize Profle</A>
}
</FONT>

<!--- The search form.  Notice we populate the SearchTerm with the variable so it refills when we search --->
<CFOUTPUT>
<FORM ACTION="search.cfm" METHOD="post">
	<INPUT TYPE="text" NAME="SearchTerm" VALUE="#HTMLEditFormat(Form.SearchTerm)#"> 
	<INPUT TYPE="submit" VALUE="Search!">
</FORM>
</CFOUTPUT>


<CFIF Len(Trim(Form.SearchTerm))>
	<!--- If there are messages to show --->
	<CFIF SearchMessages.RecordCount>
		<CFOUTPUT>
		<P>
		The text below contains your matches grouped by thread. Click the subject of the message to jump down.
		<P>
		Match<CFIF SearchMessages.RecordCount IS NOT 1>es</CFIF>: #SearchMessages.RecordCount#<P>
		<TABLE WIDTH="100%">
		</CFOUTPUT>
		
		<!--- Since we have ordered our query by threads, we use code to output the threadname. we don't want to do this for every record of our search though. we only show the thread when it changes. notice the cfif lastthread  is not thread. when it changes, we output the name, and set LASTTHREAD = thread. --->
		<CFSET LASTTHREAD="">
		<CFOUTPUT QUERY="SearchMessages">
			<CFIF LASTTHREAD IS NOT strThrd>
				<TR><TD COLSPAN="2" BGCOLOR="#application.lightgray#"><B>#strThrd#</B></TD></TR>
				<CFSET LASTTHREAD=strThrd>
			</CFIF>
			<TR>
				<TD><A HREF="##Mess#SearchMessages.CurrentRow#">#strSubject#</A></TD>
				<TD>#DateFormat(dtPosted)#</TD>
			</TR>
		</CFOUTPUT>
		

		</TABLE>
		<P>
		<HR NOSHADE SIZE="1">

		
		<CFOUTPUT QUERY="SearchMessages">
			<A NAME="Mess#SearchMessages.CurrentRow#">
			<CFINCLUDE TEMPLATE="_message_display.cfm">
		</CFOUTPUT>
		
	<CFELSE>
		<B>Sorry, no messages match your search.</B>
	</CFIF>
</CFIF>

<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">
