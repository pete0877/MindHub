<!--- ColdFusion(r) Express Global Corp. Example Application --->


<!--- If it is after or before business hours, initialize the form to make a reservation at 8am tomorrow morning, ending at 9am --->
<CFIF Hour(Now()) GT 18 and Hour(Now()) LT 8>
	<CFSET Tomorrow = DateAdd("d",1,now())>
	<CFPARAM NAME="Form.StartDate_Month" DEFAULT="#Month(Tomorrow)#">
	<CFPARAM NAME="Form.StartDate_Day" DEFAULT="#Day(Tomorrow)#">
	<CFPARAM NAME="Form.StartDate_Year" DEFAULT="#Year(Tomorrow)#">
	<CFPARAM NAME="Form.StartDate_Hour" DEFAULT="8:00">
	<CFPARAM NAME="Form.EndDate_Month" DEFAULT="#Month(Tomorrow)#">
	<CFPARAM NAME="Form.EndDate_Day" DEFAULT="#Day(Tomorrow)#">
	<CFPARAM NAME="Form.EndDate_Year" DEFAULT="#Year(Tomorrow)#">
	<CFPARAM NAME="Form.EndDate_Hour" DEFAULT="9:00">
<!--- If it is during business hours,  initialize form variables with the start date to now and end date to 1 hour from now --->	
<CFELSE>
	<CFPARAM NAME="Form.StartDate_Month" DEFAULT="#Month(Now())#">
	<CFPARAM NAME="Form.StartDate_Day" DEFAULT="#Day(Now())#">
	<CFPARAM NAME="Form.StartDate_Year" DEFAULT="#Year(Now())#">
	<CFPARAM NAME="Form.StartDate_Hour" DEFAULT="#Hour(Now())#:00">
	<CFPARAM NAME="Form.EndDate_Month" DEFAULT="#Month(Now())#">
	<CFPARAM NAME="Form.EndDate_Day" DEFAULT="#Day(Now())#">
	<CFPARAM NAME="Form.EndDate_Year" DEFAULT="#Year(Now())#">
	<CFSET OneHourFromNow = DateAdd("h",1,now())>
	<CFPARAM NAME="Form.EndDate_Hour" DEFAULT="#HOUR(OneHourFromNow)#:00">	
</CFIF>


<CFPARAM NAME="Form.Attendees" DEFAULT="">
<CFPARAM NAME="Form.RoomID" DEFAULT="">
<CFPARAM NAME="Form.EmployeeID" DEFAULT="#Client.Employee_ID#">



<!--- Get a list of rooms from the database. --->
<CFQUERY NAME="GetRooms" DATASOURCE="#application.ds#">
	SELECT 	strRmID,
			strRmName 
	FROM 	tblRooms
	ORDER BY strRmName
</CFQUERY>

<!--- Get a list of employees from the database. --->
<CFQUERY NAME="GetEmployees" DATASOURCE="#application.ds#">
	SELECT 	strEmpID,
			strFname,
			strLname,
			strEmail
	FROM 	tblEmp
	ORDER BY strLname
</CFQUERY>


<!--- Include the Global Header --->
<CFSET header.page_name="Meeting Room Reservations">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<FONT SIZE="2">
{
<A HREF="index.cfm">Meeting Room Calendar</A>
 | 
<A HREF="reservation_display.cfm">View All Reservations</A>
 | 
<A HREF="reservation_display.cfm?mode=Your">View Your Reservations</A>
}
</FONT>

<P>


<!--- Are we trying to make a reservation? --->
<CFIF IsDefined("Form.Reservation")>
	<!--- We want the results of the reservation to stick out a bit, so we make them red and bold --->
	<B>
	<!--- Our first check is to ensure a valid date. even though we have drop downs for months, days, etc., the user can still pick a date like February 31, which is not valid. Because we are defining the months, days, etc, we know that -any- error is a "days in month" type error. We begin by creating a string variable that is the combination of our date values. --->
	<CFSET StartDate=Form.StartDate_Month & "/" & Form.StartDate_Day & "/" & Form.StartDate_Year & " " & Form.StartDate_Hour>
	<CFSET EndDate=Form.EndDate_Month & "/" & Form.EndDate_Day & "/" & Form.EndDate_Year & " " & Form.EndDate_Hour & ":00">
	<CFIF NOT IsDate(StartDate)>
		<CFOUTPUT>
			
			I'm sorry, but your reservation could not be made. Your Starting Date is incorrect. 
			There are only #DaysInMonth(CreateDate("#Form.StartDate_Year#","#Form.StartDate_Month#","1"))#
			days in #MonthAsString(Form.StartDate_Month)# #Form.StartDate_Year#.

		</CFOUTPUT>
	<CFELSEIF NOT IsDate(EndDate)>
		<CFOUTPUT>

			I'm sorry, but your reservation could not be made. Your Ending Date is incorrect. 
			There are only #DaysInMonth(CreateDate("#Form.EndDate_Year#","#Form.EndDate_Month#","1"))#
			days in #MonthAsString(Form.EndDate_Month)# #Form.EndDate_Year#.

		</CFOUTPUT>
	<CFELSEIF DateCompare(ParseDateTime(StartDate),ParseDateTime(EndDate)) GT -1>
		<CFOUTPUT>

			I'm sorry, but your reservation could not be made. Your ending date is before (or the same as) your starting date.

		</CFOUTPUT>
	<CFELSE>
		<CFSET NEW_ID = CreateUUID()>
		<CFSET st = DateFormat(StartDate,"mm/dd/yyyy") & " " &  TimeFormat(StartDate)>
		<CFSET et = DateFormat(EndDate,"mm/dd/yyyy") & " " & TimeFormat(EndDate)>
		<CFQUERY NAME="AddReservation" DATASOURCE="#application.ds#">
			INSERT INTO tblRes(strResID,strRmIDFK,strEmpIDFK,dtResSDate,dtResEDate)
			VALUES('#NEW_ID#','#Form.RoomID#','#Form.EmployeeID#','#st#','#et#')
		</CFQUERY>
		<!--- Add the attendees to the table. Note - we do not care if we asked an attendee to come to a meeting and that 			    person already is busy in another meeting. --->
		<CFIF Len(Form.Attendees)>
			<CFSET Reservation_ID=NEW_ID>
			<CFLOOP INDEX="ID" LIST="#Form.Attendees#">
				<CFQUERY NAME="AddAttendees" DATASOURCE="#application.ds#">
					INSERT INTO tblAttnd(strAttndID,strResIDFK,strEmpIDFK)
					VALUES('#CreateUUID()#','#Reservation_ID#','#ID#')
				</CFQUERY>
			</CFLOOP>
		</CFIF>
		<CFOUTPUT>The reservation has been made!</CFOUTPUT>
	</CFIF>
	<CFOUTPUT></B><P></CFOUTPUT>
</CFIF>


Welcome to the Conference Room Reservation Request form. To reserve a room, please fill out the form below.
<P>

<FORM ACTION="reservation_request.cfm" METHOD="POST">
<!--- Ths hidden value serves to notify our script that we are trying to make a reservation. --->
<INPUT TYPE="hidden" NAME="Reservation" VALUE="1">
<B>Meeting Organizer: </B>
<SELECT NAME="EmployeeID">


<!--- Display every employee. --->
<CFOUTPUT QUERY="GetEmployees">
	<OPTION VALUE="#strEmpID#"<CFIF strEmpID IS FORM.EMPLOYEEID> SELECTED</CFIF>>#strFname# #strLname#</OPTION>
</CFOUTPUT>


</SELECT>
<P>
<B>Conference Room: </B>
<SELECT NAME="RoomID">

<!--- Display every room. --->
<CFOUTPUT QUERY="GetRooms">
	<OPTION VALUE="#strRmID#"<CFIF strRmID IS FORM.ROOMID> SELECTED</CFIF>>#strRmName#</OPTION>
</CFOUTPUT>


</SELECT>

<P>
<!--- The start date/time of the reservation --->
<B>Start Date/Time: </B>
<SELECT NAME="StartDate_Month">

<CFLOOP INDEX="Month" LIST="#application.months#">
	<CFSET VAL=ListFind(application.months,Month)>
	<CFOUTPUT><OPTION VALUE="#VAL#"<CFIF VAL IS FORM.STARTDATE_MONTH> SELECTED</CFIF>>#HTMLEditFormat(Month)#</CFOUTPUT>
</CFLOOP>


</SELECT>

<SELECT NAME="StartDate_Day">


<CFLOOP INDEX="X" FROM="1" TO="31">
	<CFOUTPUT><OPTION VALUE="#X#"<CFIF X IS FORM.STARTDATE_DAY> SELECTED</CFIF>>#X#</CFOUTPUT>
</CFLOOP>


</SELECT>

<SELECT NAME="StartDate_Year">


<CFLOOP INDEX="X" FROM="1999" TO="2001">
	<CFOUTPUT><OPTION<CFIF X IS FORM.STARTDATE_YEAR> SELECTED</CFIF>>#X#</CFOUTPUT>
</CFLOOP>


</SELECT>

at

<SELECT NAME="StartDate_Hour">


<!--- Display business hours --->
<CFLOOP INDEX="X" FROM="8" TO="18">
	<CFSET PROPER_HOUR=X>
	<CFSET AMPM="AM">
	<CFIF PROPER_HOUR GTE 12>
		<CFIF PROPER_HOUR GT 12>
			<CFSET PROPER_HOUR=PROPER_HOUR - 12>
		</CFIF>
		<CFSET AMPM="PM">
	</CFIF>
	<CFOUTPUT>
	<OPTION VALUE="#X#:00"<CFIF (X& ":00" ) IS FORM.STARTDATE_HOUR> SELECTED</CFIF>>#PROPER_HOUR#:00 #AMPM#
	<CFIF X LT 18>
		<OPTION VALUE="#X#:30"<CFIF (X& ":30" ) IS FORM.STARTDATE_HOUR> SELECTED</CFIF>>#PROPER_HOUR#:30 #AMPM#
	</CFIF>
	</CFOUTPUT>
</CFLOOP>


</SELECT>
	
<P>
<!--- The end date/time of the reservation --->
<B>End Date/Time: </B>
<SELECT NAME="EndDate_Month">


<CFLOOP INDEX="Month" LIST="#application.months#">
	<CFSET VAL=ListFind(application.months,Month)>
	<CFOUTPUT><OPTION VALUE="#VAL#"<CFIF VAL IS FORM.ENDDATE_MONTH> SELECTED</CFIF>>#HTMLEditFormat(Month)#</CFOUTPUT>
</CFLOOP>


</SELECT>

<SELECT NAME="EndDate_Day">


<CFLOOP INDEX="X" FROM="1" TO="31">
	<CFOUTPUT><OPTION<CFIF X IS FORM.ENDDATE_DAY> SELECTED</CFIF>>#X#</CFOUTPUT>
</CFLOOP>


</SELECT>

<SELECT NAME="EndDate_Year">


<CFLOOP INDEX="X" FROM="1999" TO="2001">
	<CFOUTPUT><OPTION<CFIF X IS FORM.ENDDATE_YEAR> SELECTED</CFIF>>#X#</CFOUTPUT>
</CFLOOP>


</SELECT>

at

<SELECT NAME="EndDate_Hour">


<!--- Display business hours --->
<CFLOOP INDEX="X" FROM="8" TO="18">
	<CFSET PROPER_HOUR=X>
	<CFSET AMPM="AM">
	<CFIF PROPER_HOUR GTE 12>
		<CFIF PROPER_HOUR GT 12>
			<CFSET PROPER_HOUR=PROPER_HOUR - 12>
		</CFIF>
		<CFSET AMPM="PM">
	</CFIF>
	<CFOUTPUT>
	<OPTION VALUE="#X#:00"<CFIF (X&":00") IS FORM.ENDDATE_HOUR> SELECTED</CFIF>>#PROPER_HOUR#:00 #AMPM#
	<CFIF X LT 18>
		<OPTION VALUE="#X#:30"<CFIF (X&":30") IS FORM.ENDDATE_HOUR> SELECTED</CFIF>>#PROPER_HOUR#:30 #AMPM#
	</CFIF>
	</CFOUTPUT>
</CFLOOP>


</SELECT>

<P>

<B>Select Attendees:</B><BR>

<!--- Display every employee. --->
<CFOUTPUT QUERY="GetEmployees">
<INPUT TYPE="checkbox" NAME="Attendees" VALUE="#strEmpID#"<CFIF LISTFIND(FORM.ATTENDEES, strEmpID)> CHECKED</CFIF>> #strFname# #strLname#<BR>
</CFOUTPUT>




<P>

<INPUT TYPE="submit" VALUE="Make My Reservation">
</FORM>





<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">