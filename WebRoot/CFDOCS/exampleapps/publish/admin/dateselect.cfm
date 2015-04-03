<CFSETTING ENABLECFOUTPUTONLY="YES">

<!--- DateSelect v1.1; all versions prior to this SHOULD NOT BE USED --->

<!--- Check for Attributes.Form --->

<CFSET ID = Attributes.SelectName>

<CFIF NOT IsDefined("Caller.DateIniFunctions")>

	<CFHTMLHEAD TEXT="<SCRIPT LANGUAGE='JAVASCRIPT'>

function DaysInMonth(currmonth,curryear) {

	if (currmonth == 2)
		if ((curryear % 4 != 0) || ((curryear % 100 == 0) && (curryear % 400 != 0)))
			return 28;
		else
			return 29;
	else if ((currmonth <= 7 && currmonth % 2 == 0) || (currmonth >= 8 && currmonth % 2 != 0))
		return 30;			
	else
		return 31;
}

function FillMonths(SelectObj) {

	var months = new Array(12);
	months[0] = 'January';
	months[1] = 'February';
	months[2] = 'March';
	months[3] = 'April';
	months[4] = 'May';
	months[5] = 'June';
	months[6] = 'July';
	months[7] = 'August';
	months[8] = 'September';
	months[9] = 'October';
	months[10] = 'November';
	months[11] = 'December';
	
	for (var i=0; i<12; i++) {
		SelectObj.options[i] = new Option(months[i],i+1);
	}

	var now = new Date();
	var currmonth = now.getMonth();
	SelectObj.options[currmonth].selected = true;
}

function FillDays(SelectObj,currmonth) {

	var totaldays = DaysInMonth(currmonth);

	for (var i=0; i<totaldays; i++) {
		SelectObj.options[i] = new Option(i+1);
	}

	var now = new Date();
	var currdate = now.getDate();
	SelectObj.options[currdate-1].selected = true;
}

function UpdateDays(SelectObj,currmonth,curryear) {

	var previousdate = SelectObj.selectedIndex;

	for (var i=SelectObj.length; i>0; i--) {
		SelectObj.options[i] = null;
	}

	var totaldays = DaysInMonth(currmonth,curryear);

	for (i=0; i<totaldays; i++) {
		SelectObj.options[i] = new Option(i+1);
	}

	if (totaldays - 1 < previousdate)
		SelectObj.options[totaldays - 1].selected = true;
	else
		SelectObj.options[previousdate].selected = true;
}

function adjustDays(SelectObj,currmonth) {
	if (SelectObj.value == '')
		SelectObj.selectedIndex = DaysInMonth(currmonth) - 1;
}

</SCRIPT>">
<CFSET Caller.DateIniFunctions = 1>
</CFIF>
<CFSETTING ENABLECFOUTPUTONLY="NO">
<NOBR>
<CFOUTPUT><SELECT NAME="#ID#Month" onChange="UpdateDays(#ID#Day,#ID#Month.selectedIndex+1,#ID#Year.value)"></CFOUTPUT>
<CFLOOP FROM="1" TO="12" INDEX="MonthNum"><CFOUTPUT>
	<CFIF Month(Now()) IS MonthNum><OPTION VALUE="#MonthNum#" SELECTED>#MonthAsString(MonthNum)#
	<CFELSE><OPTION VALUE="#MonthNum#">#MonthAsString(MonthNum)#
	</CFIF>
</CFOUTPUT></CFLOOP>
</SELECT>
<CFOUTPUT><SELECT NAME="#ID#Day" onChange="adjustDays(this,#ID#Month.selectedIndex+1)"></CFOUTPUT>
	<OPTION>00
	<OPTION>00
	<OPTION>00
	<OPTION>00
	<OPTION>00
	<OPTION>00
	<OPTION>00
	<OPTION>00
	<OPTION>00
	<OPTION>00
	<OPTION>00
	<OPTION>00
</SELECT>
<CFOUTPUT><SELECT NAME="#ID#Year" onChange="UpdateDays(#ID#Day,#ID#Month.selectedIndex+1,#ID#Year.value)"></CFOUTPUT>
<CFLOOP FROM="#Attributes.StartYear#" TO="#Attributes.EndYear#" INDEX="YearNum"><CFOUTPUT>
	<CFIF Year(Now()) IS YearNum><OPTION VALUE="#YearNum#" SELECTED>#YearNum#
	<CFELSE><OPTION VALUE="#YearNum#">#YearNum#
	</CFIF>
</CFOUTPUT></CFLOOP>
</SELECT>
</NOBR>
<CFIF IsDefined("Attributes.ShowTime")><CFIF Attributes.ShowTime IS TRUE>
<BR>
<NOBR>
<CFOUTPUT><SELECT NAME="#ID#Hour"></CFOUTPUT>
	<OPTION>1
	<OPTION>2
	<OPTION>3
	<OPTION>4
	<OPTION>5
	<OPTION>6
	<OPTION>7
	<OPTION>8
	<OPTION>9
	<OPTION>10
	<OPTION>11
	<OPTION SELECTED>12
</SELECT>
<CFOUTPUT><SELECT NAME="#ID#Minute"></CFOUTPUT>
<CFLOOP FROM="0" TO="59" INDEX="CurrHour"><CFOUTPUT>	<OPTION>#NumberFormat(CurrHour,"00")#
</CFOUTPUT></CFLOOP></SELECT>
<CFOUTPUT><SELECT NAME="#ID#AMPM"></CFOUTPUT>
	<OPTION>AM
	<OPTION>PM
</SELECT>
</NOBR>
</CFIF></CFIF>
<SCRIPT LANGUAGE="JavaScript">
<CFOUTPUT>	FillDays(document.#Attributes.Form#.#ID#Day,document.#Attributes.Form#.#ID#Month.value);</CFOUTPUT>
</SCRIPT>
