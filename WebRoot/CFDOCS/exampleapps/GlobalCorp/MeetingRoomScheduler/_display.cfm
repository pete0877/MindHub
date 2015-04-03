<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- 	This cfm acts as a fake custom tag. we cfinclude this on a page where we have already declared the values to be displayed. By using this file, we can abstract how the conference room reservation is displayed  throughout the application. --->
<CFOUTPUT>

<TR BGCOLOR="#bgcolor#">
	<TD><B>Room:</B></TD>
 	<TD>#strRmName#</TD>
</TR>
<TR BGCOLOR="#bgcolor#">
	<TD><B>Employee:</B></TD>
	<TD>#strFname# #strLname#</TD>
</TR>
<TR BGCOLOR="#bgcolor#">
	<TD><B>Begins:</B></TD>
	<TD>#DateFormat(dtResSDate,"mmmm d, yyyy")# at #TimeFormat(dtResSDate,"h:mm tt")#</TD>
</TR>
<TR BGCOLOR="#bgcolor#">
	<TD><B>Ends:</B></TD>
	<TD>#DateFormat(dtResEDate,"mmmm d, yyyy")# at #TimeFormat(dtResEDate,"h:mm tt")#</TD>
</TR>
<CFSET X=DateDiff("n",dtResSDate,dtResEDate)>
<CFSET MIN=X MOD 60>
<CFSET HOURS=(X-MIN)/60>
<TR BGCOLOR="#bgcolor#">
	<TD><B>Duration:</B></TD>
	<TD><CFIF HOURS GT 0>#HOURS# hour<CFIF HOURS GT 1>s</CFIF></CFIF> <CFIF MIN GT 0> #MIN# minutes</CFIF></TD>
</TR>


<!--- 	Do we have any attendees?  	If we do, the value is a list like this:   Ben,Ray,Joel.  	Since this is not as pretty as it could be, we use the replace function to replace ',' with ', '. this changes 	the list to:   Ben, Ray, Joel. --->
<CFIF Len(Attendees)>

	<TR BGCOLOR="#bgcolor#">
		<TD VALIGN="top"><B>Attendees:</B></TD>
		<TD>#Replace(Attendees,",",", ","ALL")#</TD>
	</TR>

</CFIF>

<P>
</CFOUTPUT>

