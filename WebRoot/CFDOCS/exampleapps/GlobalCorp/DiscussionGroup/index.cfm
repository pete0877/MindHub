<!--- ColdFusion(r) Express Global Corp. Example Application --->


<!--- Default value for the form. --->
<CFPARAM NAME="Form.Age" DEFAULT="">

<!--- Grab the threads and number of messages --->
<CFQUERY NAME="GetThreads" DATASOURCE="#application.ds#">
	SELECT 	strThrdID,
			strThrd, 
			dtCreated,
			Count(strMessID) AS MessCount,
			Max(dtPosted) as lastpost
	FROM 	tblThrd, 
			tblMes
	WHERE 	strThrdID = strThdIDFK
	GROUP BY 	strThrdID, 
				strThrd,
				dtCreated
</CFQUERY>


<!--- Include the Global Header --->
<CFSET header.page_name="Discussion Groups">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<!--- Discussion Group Menu --->
<FONT SIZE="2">
{
<A HREF="new_thread.cfm">Create New Thread</A>
 | 
<A HREF="profile.cfm">Customize Profle</A>
 | 
<A HREF="search.cfm">Search Messages</A>
}
</FONT>

<P>
<B>Welcome to the Discussion Groups</B>
<BR>
To limit the number of threads displayed, choose a filter here: 
<FORM ACTION="index.cfm" METHOD="POST">
<SELECT NAME="Age">
<!--- Notice how we check for an existing Form value --->
<OPTION VALUE=""<CFIF FORM.AGE IS "">SELECTED</CFIF>>All Threads
<OPTION VALUE="7"<CFIF FORM.AGE IS "7">SELECTED</CFIF>>At most 7 days old
<OPTION VALUE="14"<CFIF FORM.AGE IS "14">SELECTED</CFIF>>At most 14 days old
<OPTION VALUE="30"<CFIF FORM.AGE IS "30">SELECTED</CFIF>>At most 30 days old
<OPTION VALUE="90"<CFIF FORM.AGE IS "90">SELECTED</CFIF>>At most 90 days old
</SELECT> 
<INPUT TYPE="submit" VALUE="Filter">
</FORM>


<P>

<HR NOSHADE SIZE="1">


<!--- Display Threads --->
<CFIF GetThreads.RecordCount IS 0>
	<B>There are no threads currently</B>
<CFELSE>

	<TABLE WIDTH="100%">
		<TR>
			<TD><B>Thread</B></TD>
			<TD><B>Messages</B></TD>
			<TD><B>Last Post</B></TD>
		</TR>
		<CFOUTPUT QUERY="GetThreads">
			<CFIF NOT VAL(Form.Age) OR (Val(Form.Age) AND DateDiff("d",dtCreated,Now()) LTE Form.Age)>	
			<TR>
				<TD><A HREF="view_thread.cfm?id=#strThrdID#">#strThrd#</A></TD>
				<TD>#MessCount#</TD>
				<TD>#DateFormat(LastPost,"m/d/yy")#</TD>
			</TR>
			</CFIF>
		</CFOUTPUT>
	</TABLE>
</CFIF>

<HR NOSHADE SIZE="1">

<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">
