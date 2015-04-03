<CFSET Title = "Under the Hood, Pt. 1">
<CFINCLUDE TEMPLATE="_header.cfm">

<h3>Under the Hood, Pt. I</h3>
<P>If you just want to poke around with the example application and see what a 
  content publishing system looks like, we've covered about everything you need 
  to know. Keep reading if you want to understand what's happening behind the 
  scenes. </P>
<P><IMG SRC="schema.gif" WIDTH=516 HEIGHT=217 BORDER=0 ALT="Database Schema"></P>
<P>In the CFexamples datasource, there are eight tables that begin with Pub. These 
  are the tables that are associated with this example application. (For clarity, 
  PubUsers and PubClassDefinitions are not shown in the graphic above.)</P>
<P>Every object has an entry in <B>PubObjects</B>, which (among other things) 
  specifies which of the classes in <B>PubDataClasses</B> the object belongs to.</P>
<P>Each object has one related record in <B>PubContent</B> for <I>each</I> piece 
  of content in that object. All of the PubContent records relate to one of the 
  PubContentTypes (<I>one</I> PubContentTypes record to <I>many</I> PubContent 
  records).</P>
<P>There is one record in <B>PubInstances</B> for each instance of each object, 
  and every page that has instances scheduled on it has a record in <B>PubPages</B>; 
  each instance in PubInstances points to one of the pages in PubPages.</P>

<CFSET HREF = "4-under2.cfm">
<CFSET Link = "Under the Hood, Pt. 2">
<CFINCLUDE TEMPLATE="_footer.cfm">