<CFSET Title = "Learning More">
<CFINCLUDE TEMPLATE="_header.cfm">

<H3>Learning More</H3>

<P>If you've read all the way up to this point, you've seen the foundation of
the dynamic content publishing system we've created. Now you're ready to browse
through the DCP system on your own, creating objects and instances, uploading
files, etc.</P>

<H4>Security Note</H4>

<P>One thing to note is that the DCP Administrator will only respond to browser
calls to/from 127.0.0.1, for security purposes. Since files can be freely uploaded
with this system, a visitor could upload a .cfm template with malicious code and
execute it from your server. By restricting calls to 127.0.0.1, a hacker would
have to be actually sitting in front of the server to access the DCP Administrator
at all.</P>

<P>If it's impossible for you to use the DCP Administrator from the server, you can
remove the 127.0.0.1 restriction by editing publish/admin/application.cfm and changing
the line:</P>

<BLOCKQUOTE><PRE>&lt;CFIF CGI.REMOTE_ADDR NEQ &quot;127.0.0.1&quot;&gt;</PRE></BLOCKQUOTE>

<P>to something like:</P>

<BLOCKQUOTE><PRE>&lt;CFIF CGI.REMOTE_ADDR IS &quot;0&quot;&gt;</PRE></BLOCKQUOTE>

<P>Because this leaves your system vulnerable, you may want to at least rename the
"publish" directory to something less obvious.</P>

<H4>That's All...</H4>

<P>That's the end of the DCP documentation; now go out and play with the system. And don't
be afraid to dive into some of the CFML source. We hope you find this content publishing
system useful!</P>

<ul>
	<li><a href="../../publish/admin/index.cfm">Enter as an adminstrator</a>
	<li><a href="../../publish/welcome.cfm">View the pages as an end-user</a>
</ul>

<CFINCLUDE TEMPLATE="_footer.cfm">