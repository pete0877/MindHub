<CFSETTING ENABLECFOUTPUTONLY="YES">


<!--- Initialize variables --->

<CFPARAM NAME="Attributes.BreakChars" DEFAULT=" -">
<CFSET BreakChars = Attributes.BreakChars>

<CFPARAM NAME="Attributes.Width" DEFAULT="80">
<CFSET Width = Attributes.Width>

<CFSET Newline = Chr(10)>

<CFSET Wrapped = "">

<CFSET Remaining =  evaluate("Caller." & Attributes.Variable)>



<!--- Loop until there are fewer than #WIDTH# characters left --->

<CFLOOP CONDITION="Len(Remaining) GT Width">

	<CFSET CurrentLine = Left(Remaining, Width)>
	<CFSET Remaining = Right(Remaining, Len(Remaining) - Width)>

	
	<CFIF Find(Newline, CurrentLine) NEQ 0>
		<CFSET Pos = Find(Newline, CurrentLine)>
		<CFSET Wrapped = Wrapped & Left(CurrentLine, Pos)>
		<CFIF Pos NEQ Len(CurrentLine)>
			<CFSET Remaining = Right(CurrentLine, Len(CurrentLine) - Pos) & Remaining>
		</CFIF>

	<CFELSEIF FindOneOf(BreakChars, CurrentLine) NEQ 0>
		<CFSET CurrentLine = Reverse(CurrentLine)>
		<CFSET Pos = FindOneOf(BreakChars, CurrentLine)>
		<CFSET CurrentLine = Reverse(CurrentLine)>
		<CFSET Pos = Len(CurrentLine) - Pos + 1>

		<CFSET Wrapped = Wrapped & Left(CurrentLine, Pos) & Newline>
		<CFIF Pos NEQ Len(CurrentLine)>
			<CFSET Remaining = Right(CurrentLine, Len(CurrentLine) - Pos) & Remaining>
		</CFIF>

	<CFELSE>
		<CFSET Wrapped = Wrapped & CurrentLine & Newline>

	</CFIF>

</CFLOOP>

<CFSET Wrapped = Wrapped & Remaining>

<CFSET "Caller.#Attributes.Variable#" = Wrapped>

<CFSETTING ENABLECFOUTPUTONLY="NO">