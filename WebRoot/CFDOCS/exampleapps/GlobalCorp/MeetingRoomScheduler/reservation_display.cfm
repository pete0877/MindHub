<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- We use CFPARAM to create default values --->
<CFPARAM NAME="Form.Employee_ID" DEFAULT="Anyone">
<CFPARAM NAME="Form.DisplayRange" DEFAULT="Anytime">

<!--- Grab the Employees --->
<CFQUERY NAME="GetEmployees" DATASOURCE="#application.ds#">
	SELECT strEmpID,strFname,strLname
	FROM tblEmp
</CFQUERY>

<!--- We have an option to enter this page from index.cfm. when we come in, we are arriving with info on our url string. we are going to check for a specific url parameter, and if it exists, were going to set it = to the form value. This means we can keep using the cfswitch based on the form value that this page uses. --->
<CFIF IsDefined("URL.mode")>
	<CFSET Form.DisplayRange=URL.mode>
</CFIF>

<!--- We need to do some special processing if the user asked for a specific date range. The nice thing is that we can simply create 2 date obs to send to sql to make sure the reservation is within them. the CFSWITCH statement below will create the obs depending on what you requested. --->
<CFIF Form.DisplayRange IS NOT "Anytime">

	<CFSWITCH EXPRESSION="#Form.DisplayRange#">
	
		<CFCASE VALUE="This Week">
			<!--- What day of the week is it? figure out how many days from Sunday that is, and datediff from then to get to Sunday. --->
			<CFSET TodaysWeekDay=DayOfWeek(Now())>
			<CFSET Difference=TodaysWeekDay - 1>
			<CFSET BeginDate=DateAdd("d",-Difference,Now())>
			<!--- Now create a day for Saturday. We can get it pretty easily by doing a date add to BeginDate --->
			<CFSET EndDate=DateAdd("d",6,BeginDate)>
		</CFCASE>
		
		<CFCASE VALUE="Next Week">
			<!--- Find the next Sunday. --->
			<CFSET TodaysWeekDay=DayOfWeek(Now())>
			<CFSET ToAdd=8 - TodaysWeekDay>
			<CFSET BeginDate=DateAdd("d",ToAdd,Now())>
			<!--- The End date is simple, we just add 6 to begindate. --->
			<CFSET EndDate=DateAdd("d",6,BeginDate)>
		</CFCASE>
		
		<CFCASE VALUE="Next Two Weeks">
			<!--- This is easy as well. We just combine the previous two cases. --->
			<CFSET TodaysWeekDay=DayOfWeek(Now())>
			<CFSET Difference=TodaysWeekDay - 1>
			<CFSET BeginDate=DateAdd("d",-Difference,Now())>
			<CFSET ToAdd=8 - TodaysWeekDay>
			<CFSET TempDate=DateAdd("d",ToAdd,Now())>
			<!--- The End date is simple, we just add 6 to begindate. --->
			<CFSET EndDate=DateAdd("d",6,TempDate)>
		</CFCASE>
		
		<CFCASE VALUE="This Month">
			<!--- Easy. Figure out this month. Then make new dates for day 1 of the month, and the daysinmonth of this month. --->
			<CFSET BeginDate=CreateDate(Year(Now()),Month(Now()),1)>
			<CFSET EndDate=CreateDate(Year(Now()),Month(Now()),DaysInMonth(Now()))>
		</CFCASE>
		
		<CFCASE VALUE="Next Month">
			<!--- Same code as last time, except we add 1 to Month --->
			<CFSET NextMonth=DateAdd("m",1,Now())>
			<CFSET BeginDate=CreateDate(Year(Now()),Month(NextMonth),1)>
			<CFSET EndDate=CreateDate(Year(Now()),Month(NextMonth),DaysInMonth(NextMonth))>
		</CFCASE>
		
		<CFCASE VALUE="Next Two Months">
			<!--- Again, this is a simple combo of the previous 2 cases --->
			<CFSET BeginDate=CreateDate(Year(Now()),Month(Now()),1)>
			<CFSET NextMonth=DateAdd("m",1,Now())>
			<CFSET EndDate=CreateDate(Year(Now()),Month(NextMonth),DaysInMonth(NextMonth))>
		</CFCASE>
		
		<CFCASE VALUE="month">
			<!--- Valid values in the url --->
			<CFIF Val(URL.month) LTE 12 AND Val(URL.month) GTE 1 AND Val(URL.year) LTE 9998>
				<CFSET BeginDate=CreateDate(URL.year,URL.month,1)>
				<CFSET EndDate=CreateDate(URL.year,URL.month,DaysInMonth(BeginDate))>
			</CFIF>
		</CFCASE>

		<CFCASE VALUE="day">
			<!--- Valid values in the url --->
			<CFIF Val(URL.month) LTE 12 AND Val(URL.month) GTE 1 AND Val(URL.year) LTE 9998>
				<CFSET TempDate=CreateDate(URL.year,URL.month,1)>
				<CFIF URL.day LTE DaysInMonth(TempDate)>
					<CFSET BeginDate=CreateDate(URL.year,URL.month,URL.day)>
					<CFSET num_mins_in_day=24*60>
					<CFSET EndDate=DateAdd("n",num_mins_in_day,CreateDate(URL.year,URL.month,URL.day))>
				</CFIF>
			</CFIF>
		</CFCASE>
		
		<CFCASE VALUE="your">
			<CFSET Form.Employee_ID=Client.Employee_ID>
		</CFCASE>
		
	</CFSWITCH>
</CFIF>

<CFQUERY NAME="GetRes" DATASOURCE="#application.ds#">
	SELECT strResID, dtResSDate, dtResEDate, strFname, strRmName, strLname
	FROM   tblRes, tblRooms, tblEmp 
	WHERE  strRmIDFK = strRmID AND
           strEmpIDFK = strEmpID
		   <CFIF Form.Employee_ID IS NOT "Anyone">
		   AND strEmpID = '#Form.Employee_ID#'
		   </CFIF>
	ORDER BY	strRmName
</CFQUERY>

<!--- We need to do the date filtering here because of the problem we had in dBase --->
<CFSET GetReservations = QueryNew(GetRes.columnlist)>
<CFSET counter = 0>
<CFLOOP QUERY="GetRes">
	<!--- First filter is on res that have passed --->
	<CFSET DatePart = ListFirst(dtResSDate," ")>
	<CFSET Year = ListLast(DatePart,"/")>
	<CFSET Month = ListFirst(DatePart,"/")>
	<CFSET Day = ListGetAt(DatePart,2,"/")>
	<CFSET DateOb = CreateDate(Year,Month,Day)>
	<CFSET FirstMon = CreateDate(Year(Now()),Month(Now()),1)>
	<CFIF DateDiff("s",FirstMon,DateOb) GT 0>
		<CFIF Form.DisplayRange IS NOT "Anytime" AND Form.DisplayRange IS NOT "your">
			<CFIF BeginDate LTE DateOb AND DateOb LTE EndDate>
				<CFSET counter = counter + 1>
				<CFSET QueryAddRow(GetReservations,1)>
				<CFLOOP INDEX="field" LIST="#GetRes.columnlist#">
					<CFSET QuerySetCell(GetReservations,"#field#","#Evaluate(field)#")>
				</CFLOOP>
			</CFIF>
		<CFELSE>
			<CFSET counter = counter + 1>
			<CFSET QueryAddRow(GetReservations,1)>
			<CFLOOP INDEX="field" LIST="#GetRes.columnlist#">
				<CFSET QuerySetCell(GetReservations,"#field#","#Evaluate(field)#")>
			</CFLOOP>
		</CFIF>
	</CFIF>	
</CFLOOP>

<!--- Include the Global Header --->
<CFSET header.page_name="View Meeting Room Reservations">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">



<!--- Meeting Room menu --->
<FONT SIZE="2">
{
<A HREF="index.cfm">Meeting Room Calendar</A>
 | 
<!--- If we are not showing Your Reservations, make a link to do it --->
<CFIF Form.DisplayRange is not "your">
	<A HREF="reservation_display.cfm?mode=your">View Your Reservations</A>
<!--- If we are showing Your Reservations, make a link to show All Reservations --->
<CFELSE>
	<A HREF="reservation_display.cfm">View All Reservations</A>
</CFIF> 
 | 
<A HREF="reservation_request.cfm">Request a Reservation</A>
}
</FONT>





<FORM ACTION="reservation_display.cfm" METHOD="POST">
<B>Limit reservations to those by:</B>
<BR>
Employee: 
<SELECT NAME="Employee_ID">
	<OPTION VALUE="Anyone">Anyone
	<!--- In the code below, notice how we check if the form value is equal to the current employee_id. this allows our code to "remember" what the user picked last time. --->
	<CFOUTPUT QUERY="GetEmployees">
	<OPTION VALUE="#strEmpID#" <CFIF FORM.EMPLOYEE_ID IS strEmpID>SELECTED</CFIF>>#strFname# #strLname#
	</CFOUTPUT>
</SELECT> 
<BR>
Date Range: 
<SELECT NAME="DisplayRange">
	<!--- Like above, we check for an existing value. --->
	<CFLOOP INDEX="CurrentRange" LIST="#application.displayrangelist#">
		<CFOUTPUT><OPTION VALUE="#CurrentRange#" <CFIF CURRENTRANGE IS FORM.DISPLAYRANGE>SELECTED</CFIF>>#CurrentRange#</CFOUTPUT>
	</CFLOOP>
</SELECT>
<BR>
<INPUT TYPE="Submit" VALUE="Limit Display">
</FORM>

<HR NOSHADE>

<!--- If there are any reservations to show --->
<CFIF GetReservations.RecordCount>
	<TABLE BORDER="0" CELLSPACING="0" CELLPADDING="2">
	<CFOUTPUT QUERY="GetReservations">
		<!--- Run a query to see if we have any attendees --->
		<CFQUERY NAME="GetAttendees" DATASOURCE="#application.ds#">
			SELECT 	strEmpIDFK,strFname,strLname
			FROM 	tblAttnd,
					tblEmp
			WHERE  	strResIDFK = '#strResID#' 
			AND 	strEmpIDFK = strEmpID
		</CFQUERY>
		<CFSET Attendees="">
		<CFIF GetAttendees.RecordCount GT 0>
			<CFLOOP QUERY="GetAttendees">
				<CFSET Attendees = ListAppend(Attendees,strFname & " " & strLname)>
			</CFLOOP>
		</CFIF>
		<!--- Alternate colors --->
		<CFSET BGCOLOR = IIF(CurrentRow MOD 2,"application.lightgray","application.white")>
		<!--- This page is the display for each reservation to keep it consistent. --->
		<CFINCLUDE TEMPLATE="_display.cfm">
		<!--- An extra row for spacing between events --->
		<TR>
		<TD>&nbsp;</TD><TD>&nbsp;</TD>
		</TR>
	</CFOUTPUT>
	</TABLE>
<!--- If there are no reservations --->
<CFELSE>
	<B>No Current Reservations to Display</B>
</CFIF>





<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">