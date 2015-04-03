<!--- This template takes an EmpID and shows the details for that employee --->

<!--- If we weren't given an EmpID, this template can't be
	processed; go instead to the employee list --->
<CFIF NOT ISDEFINED("URL.EmpID")>
	<CFLOCATION URL="List.cfm">
</CFIF>

<!--- This includes the HTML that goes into the header. --->
<CFINCLUDE TEMPLATE="_header.cfm">

<!--- Retrieve the employee's details using a SQL query --->
<CFQUERY datasource="cfx" NAME="GetEmployee">
SELECT * FROM tblEmp, tblDpt
WHERE strDptIDFK = strDptID
	AND strEmpID = '#URL.EmpID#'
</CFQUERY>

<!--- Now we just output the results --->
<CFOUTPUT QUERY="GetEmployee">
<TR>
   	<TD WIDTH=100 ALIGN="RIGHT" VALIGN="TOP" HEIGHT="150">&nbsp;</TD>
	<TD VALIGN="TOP">
		<FONT FACE="Arial, Helvetica">
		<P>
			<B>Name:</B> #strFName# #strLName#<BR>
			<B>Title:</B> #strTitle#<BR>
			<B>Department:</B> #strDptName#<BR>
			<B>Temporary:</B> #YesNoFormat(binIsTemp)#<BR>
			<B>E-mail:</B> #strEmail#<BR>
			<B>Phone:</B> #strPhone#<BR>
			<B>Start Date:</B> #DateFormat(dtStrtDate, "mmmm d, yyyy")#<BR>
		</P>

		<P><FONT SIZE="2"><I>#glbPersnl#</I></FONT></P>
        
        <!--- Notice we're passing the EmpID in the URL... --->
		<P><FONT SIZE="2"><A HREF="modifyform.cfm?EmpID=#strEmpID#">Modify Information</A></FONT></P>
	</TD>
	<TD WIDTH="100" NOWRAP>&nbsp;</TD>
</TR>
</CFOUTPUT>

<TR>
  	<TD HEIGHT="75" COLSPAN="3">&nbsp;</TD>
</TR>
</TABLE>

<!--- Close the document --->
<CFINCLUDE TEMPLATE="_footer.cfm">