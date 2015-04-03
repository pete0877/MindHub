<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- Include the Global Header --->
<CFSET header.page_name="Meeting Room Scheduler">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<CFPARAM NAME="URL.startmonth" DEFAULT="#Month(Now())#">
<CFPARAM NAME="URL.startyear" DEFAULT="#Year(Now())#">
<!--- If someone messes with the url, this will repair it --->
<CFIF Val(URL.startmonth) LTE 0 OR Val(URL.startmonth) GTE 13 OR Val(URL.startyear) GTE 9999>
	<CFSET URL.startmonth=Month(Now())>
	<CFSET URL.startyear=Year(Now())>
</CFIF>



<!--- Meeting Room Menu --->
<FONT SIZE="2">
{
<A HREF="reservation_display.cfm">View All Reservations</A>
 | 
<A HREF="reservation_display.cfm?mode=Your">View Your Reservations</A>
 | 
<A HREF="reservation_request.cfm">Request a Reservation</A> 
}
</FONT>

<P>
<B>Choose a date from the calendar below.</B>
<P>


<CENTER>
<TABLE CELLPADDING="5">
	<TR VALIGN="top" ALIGN="center">
		<!--- Month 1 --->
		<!--- Do a query to grab the reservation ids for the month. --->
		<CFQUERY NAME="GetResInMonth" DATASOURCE="#application.ds#">
			SELECT dtResSDate AS eday
			FROM   tblRes
		</CFQUERY>
		<!--- 
			Recreate the query to strip out for proper month. We need do this because
		    of date/time problem in dBase. We store the res start and end times as strings now.
		--->
		<CFSET event_datelist = "">
		<CFLOOP QUERY="GetResInMonth">
			<CFSET DatePart = ListFirst(eday," ")>
			<CFSET Year = ListLast(DatePart,"/")>
			<CFSET Month = ListFirst(DatePart,"/")>
			<CFIF Month IS Month(Now()) AND Year IS Year(Now())>
				<CFSET Day = ListGetAt(DatePart,2,"/")>
				<CFSET event_datelist = ListAppend(event_datelist,Day)>
			</CFIF>
		</CFLOOP>
		<CFSET dateob=CreateDate(URL.startyear,URL.startmonth,1)>
		<TD><CFINCLUDE TEMPLATE="_calendar_display.cfm"></TD>
		<!--- Month 2 --->
		<!--- Do a query to grab the reservation ids for the month. --->
		<CFQUERY NAME="GetResInMonth2" DATASOURCE="#application.ds#">
			SELECT dtResSDate AS eday
			FROM   tblRes
		</CFQUERY>
		<CFSET event_datelist = "">
		<CFLOOP QUERY="GetResInMonth2">
			<CFSET DatePart = ListFirst(eday," ")>
			<CFSET Year = ListLast(DatePart,"/")>
			<CFSET Month = ListFirst(DatePart,"/")>
			<CFSET NewMonth = DateAdd("m",1,dateob)>
			<CFIF Month IS Month(NewMonth) AND Year IS Year(NewMonth)>
				<CFSET Day = ListGetAt(DatePart,2,"/")>
				<CFSET event_datelist = ListAppend(event_datelist,Day)>
			</CFIF>
		</CFLOOP>
		<CFSET dateob=DateAdd("m",1,dateob)>
		<TD><CFINCLUDE TEMPLATE="_calendar_display.cfm"></TD>
	</TR>
</TABLE>
<CFSET prev_date=DateAdd("m",-3,dateob)>
<CFSET next_date=DateAdd("m",1,dateob)>
<CFOUTPUT>
<A HREF="index.cfm?startmonth=#Month(prev_date)#&startyear=#Year(prev_date)#">Last 2 Months</A> ~ <A HREF="index.cfm?startmonth=#Month(next_date)#&startyear=#Year(next_date)#">Next 2 Months</A>
</CFOUTPUT>
</CENTER>

<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">


