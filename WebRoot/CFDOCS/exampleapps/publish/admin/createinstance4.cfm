<!--- This template will fetch all the other instances that coincide with this
	one and allow the user to set a priority accordingly --->

<!--- First let's do something with these scheduling variables --->

<CFIF StartType IS 0>
	<CFSET StartDateTime = "">
<CFELSE>
	<!--- This CFIF block is just to covert to 24hr format --->
	<CFIF StartAMPM IS "PM">
		<CFSET TempHour = StartHour + 12>
	<CFELSE>
		<CFSET TempHour = StartHour>
	</CFIF>
	<CFSET StartDateTime = CreateDateTime(StartYear, StartMonth, StartDay, TempHour, StartMinute, 0)>
</CFIF>

<CFIF EndType IS 0>
	<CFSET EndDateTime = "">
<CFELSE>
	<!--- This CFIF block is just to covert to 24hr format --->
	<CFIF EndAMPM IS "PM">
		<CFSET TempHour = EndHour + 12>
	<CFELSE>
		<CFSET TempHour = EndHour>
	</CFIF>
	<CFSET EndDateTime = CreateDateTime(EndYear, EndMonth, EndDay, TempHour, EndMinute, 0)>
</CFIF>

<!--- Find all the instances that would ever appear with this one --->
<CFQUERY datasource="cfx" NAME="GetRelevantInstances">
SELECT * FROM tblPInt pint, tblPbCnt cnt
WHERE pint.strObjIDFK = cnt.strObjIDFK
	AND cnt.strTypIDFK = '#Application.HeadLineTypeID#'
	AND strPgIDFK = '#PageID#'
	AND intLoc = #Location#
	<CFIF NOT StartDateTime IS "">
		AND (dtEndTm >= #StartDateTime# OR dtEndTm = Null)
	</CFIF>
	<CFIF NOT EndDateTime IS "">
		AND (dtStrtTm <= #EndDateTime# OR dtStrtTm = Null)
	</CFIF>
ORDER BY intPrior DESC
</CFQUERY>

<CFIF GetRelevantInstances.RecordCount NEQ 0>
	<!--- Find out what Content Type 1 is called --->
	<CFQUERY datasource="cfx" NAME="GetTypeName">
	SELECT * FROM tblPbCTp
	WHERE strTypID = '#Application.HeadLineTypeID#'
	</CFQUERY>
</CFIF>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Add Instance</TITLE>
<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>
</HEAD>

<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">

<TABLE ALIGN="RIGHT" CELLPADDING="0" CELLSPACING="0">
<TR>
	<TD ALIGN="RIGHT">
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		<A HREF="index.cfm">Administrator Home</A><BR>
		<CFOUTPUT><A HREF="properties.cfm?ObjectID=#ObjectID#">Return to Object #ObjectID#</A></CFOUTPUT>
	</FONT>
	</TD>
</TR>
</TABLE>

<P><IMG SRC="images/createinstance.gif" WIDTH=131 HEIGHT=20 BORDER=0 ALT="Create Instance"></P>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><CFOUTPUT>Creating an instance for <B>Object #ObjectID#</B> on <B>#TemplatePath#</B>, in <B>Location #Location#</B>...</CFOUTPUT></P>

<BR>

<FORM ACTION="createinstance5.cfm" METHOD="POST">

<CFOUTPUT>
	<INPUT TYPE="HIDDEN" NAME="strObjIDFK" VALUE="#ObjectID#">
	<INPUT TYPE="HIDDEN" NAME="strPgIDFK" VALUE="#PageID#">
	<INPUT TYPE="HIDDEN" NAME="intLoc" VALUE="#Location#">
	<INPUT TYPE="HIDDEN" NAME="dtStrtTm" VALUE="#StartDateTime#">
	<INPUT TYPE="HIDDEN" NAME="dtEndTm" VALUE="#EndDateTime#">
	<INPUT TYPE="HIDDEN" NAME="strInsID" VALUE="#CreateUUID()#">
</CFOUTPUT>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Priority:</B></FONT><BR>
<INPUT TYPE="TEXT" NAME="intPrior" SIZE="4" VALUE="100">
</P>

<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

<CFIF GetRelevantInstances.RecordCount IS 0>

	<TABLE BORDER="1" CELLPADDING="5" CELLSPACING="2">
	<TR>
		<TD>
		<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		No other objects are scheduled to appear on this page, in this<BR>
		location, at the same time as this object. You may assign the<BR>
		priority of this instance to any value.</FONT></P>
		</TD>
	</TR>
	</TABLE>

<CFELSE>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
	<CFOUTPUT>Listed below are object(s) which are scheduled to appear on<BR>
	page <B>#TemplatePath#</B> in location <B>#Location#</B> at the same time as the<BR>
	instance you are scheduling.</CFOUTPUT>
	</FONT></P>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
	Please set this instance's priority to determine the order in<BR>
	which they should appear (higher numbers will appear first).
	</FONT></P>

	<TABLE BORDER="1" CELLPADDING="5" CELLSPACING="2">
	<TR>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>ObjectID</B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Start Time</B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>End Time</B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B><CFOUTPUT>#GetTypeName.strTypName#</CFOUTPUT></B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Priority</B></FONT></TD>
	</TR>
	<CFOUTPUT QUERY="GetRelevantInstances">
	<TR>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#strObjIDFK#</FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
			<CFIF dtStrtTm IS "">Beginning of time
			<CFELSE>#dtStrtTm#</CFIF>
			</FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
			<CFIF dtEndTm IS "">End of time
			<CFELSE>#dtEndTm#</CFIF>
			</FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#glbData#</FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#intPrior#</FONT></TD>
	</TR>
	</CFOUTPUT>
	</TABLE>
	
</CFIF>

</FORM>

</BODY>
</HTML>

<CFHEADER NAME="Expires" VALUE="#Now()#">