<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- 	This CFM acts as a fake custom tag. we cfinclude this on a page where we have already declared the values to be displayed. by using this file, we can abstract our calendar functionality. --->

<CFOUTPUT>
<TABLE WIDTH="140" CELLPADDING="3" BORDER="1">
<TR BGCOLOR="#application.table_headcolor#">
<TD COLSPAN="7" ALIGN="center">
<CFIF Len(event_datelist)><A HREF="reservation_display.cfm?mode=month&month=#Month(dateob)#&year=#Year(dateob)#"></CFIF>
<B>#MonthAsString(Month(DateOb))# #Year(Dateob)#</B>
<CFIF Len(event_datelist)></A></CFIF>
</TD>
</TR>
</CFOUTPUT>

<!--- Now we need to display the weeks of the month. --->
<!---  The logic here is not too complex. We know that every 7 days we need to start a new table row. The only hard part is figuring out how much we need to pad the first and last row. To figure out how much we need to pad, we just figure out what day of the week the first of the month is. if it is wednesday, then we need to pad for sunday,monday, and tuesday. 3 days. --->

<TR>
<CFSET FIRSTOFMONTH=CreateDate(Year(DateOb),Month(DateOb),1)>
<CFSET TOPAD=DayOfWeek(FIRSTOFMONTH) - 1>
<CFSET PADSTR=RepeatString("<TD>&nbsp;</TD>",TOPAD)>
<CFOUTPUT>#PADSTR#</CFOUTPUT>
<CFSET DW=TOPAD>
<CFLOOP INDEX="X" FROM="1" TO="#DaysInMonth(DateOb)#">
	
	<CFIF ListFind(event_datelist,X)>
		<CFOUTPUT><TD BGCOLOR="#application.table_cellcolor#"><A HREF="reservation_display.cfm?mode=day&day=#X#&month=#Month(dateob)#&year=#Year(dateob)#">#X#</A></CFOUTPUT>
	<CFELSE>
		<TD><CFOUTPUT>#X#</CFOUTPUT>
	</CFIF>
	</TD>
	<CFSET DW=DW + 1>
	<CFIF DW EQ 7>
		</TR>
		<CFSET DW=0>
		<CFIF X LT DaysInMonth(DateOb)><TR></CFIF>
	</CFIF>
</CFLOOP>
<!---
	Now we need to do a pad at the end, just to make our table "proper"  we can figure out how much the pad should be by examining DW --->
<CFSET TOPAD=7 - DW>
<CFIF TOPAD LT 7>
	<CFSET PADSTR=RepeatString("<TD>&nbsp;</TD>",TOPAD)>
	<CFOUTPUT>#PADSTR#</CFOUTPUT>
</CFIF>
</TR>
</TABLE>

