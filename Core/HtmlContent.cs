using System;
using System.Linq;

namespace Greatbone.Core
{
    /// <summary>
    /// For dynamic HTML5 content tooled with Zurb Foundation
    /// </summary>
    public class HtmlContent : DynamicContent, IDataOutput<HtmlContent>
    {
        readonly ActionContext ac;

        // counts of each level
        readonly int[] counts = new int[8];

        // current level
        int level;

        // type of outputting
        sbyte kind;

        internal const sbyte JS = 1, UL = 2;

        public HtmlContent(ActionContext ac, bool octet, int capacity = 32 * 1024) : base(octet, capacity)
        {
            this.ac = ac;
        }

        public override string Type => "text/html; charset=utf-8";

        void AddLabel(string label, string alt)
        {
            if (label != null)
            {
                Add(label);
            }
            else // alt uppercase
            {
                for (int i = 0; i < alt.Length; i++)
                {
                    char c = alt[i];
                    if (c >= 'a' && c <= 'z')
                    {
                        c = (char) (c - 32);
                    }
                    Add(c);
                }
            }
        }

        public void AddEsc(string v)
        {
            if (v == null) return;

            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (c == '<')
                {
                    Add("&lt;");
                }
                else if (c == '>')
                {
                    Add("&gt;");
                }
                else if (c == '&')
                {
                    Add("&amp;");
                }
                else if (c == '"')
                {
                    Add("&quot;");
                }
                else
                {
                    Add(c);
                }
            }
        }

        void AddJsonEsc(string v)
        {
            if (v == null) return;

            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (c == '\"')
                {
                    Add('\\');
                    Add('"');
                }
                else if (c == '\\')
                {
                    Add('\\');
                    Add('\\');
                }
                else if (c == '\n')
                {
                    Add('\\');
                    Add('n');
                }
                else if (c == '\r')
                {
                    Add('\\');
                    Add('r');
                }
                else if (c == '\t')
                {
                    Add('\\');
                    Add('t');
                }
                else
                {
                    Add(c);
                }
            }
        }

        public HtmlContent T(string str)
        {
            Add(str);
            return this;
        }

        public HtmlContent T(char v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(short v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(int v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(long v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(DateTime v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(decimal v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(double v)
        {
            Add(v);
            return this;
        }

        public HtmlContent SP(string str)
        {
            Add("&nbsp;");
            Add(str);
            return this;
        }

        public HtmlContent SP(char v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent SP(short v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent SP(int v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent SP(long v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent SP(DateTime v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent SP(decimal v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent SP(double v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent TH(string label)
        {
            Add("<th>");
            Add(label);
            Add("</th>");
            return this;
        }

        public HtmlContent TH_()
        {
            Add("<th>");
            return this;
        }

        public HtmlContent _TH()
        {
            Add("</th>");
            return this;
        }

        public HtmlContent TD(bool v)
        {
            Add("<td style=\"text-align: center\">");
            if (v)
            {
                Add("&radic;");
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TD(short v, bool zero = false)
        {
            Add("<td style=\"text-align: right\">");
            if (v != 0 || zero)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TD(int v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(long v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(decimal v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(DateTime v)
        {
            Add("<td>");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(string v)
        {
            Add("<td>");
            AddEsc(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(string v1, string v2)
        {
            Add("<td>");
            AddEsc(v1);
            Add("&nbsp;");
            AddEsc(v2);
            Add("</td>");
            return this;
        }

        public HtmlContent TD_(short v)
        {
            Add("<td>");
            Add(v);
            return this;
        }

        public HtmlContent TD_(int v)
        {
            Add("<td>");
            Add(v);
            return this;
        }

        public HtmlContent TD_(string v = null)
        {
            Add("<td>");
            if (v != null)
            {
                AddEsc(v);
            }
            return this;
        }

        public HtmlContent TD_(double v)
        {
            Add("<td>");
            Add(v);
            return this;
        }

        public HtmlContent _TD()
        {
            Add("</td>");
            return this;
        }

        public HtmlContent COL(string label, short v, int n = 0)
        {
            Add("<div class=\"grid-x grid-padding-x\">");
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label);
            Add("：</div>");
            Add("<div class=\"cell small-9\">");
            Add(v);
            Add("</div>");
            Add("</div>");
            return this;
        }

        public HtmlContent COL(string label, int v, int n = 0)
        {
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label);
            Add("：</div>");
            Add("<div class=\"cell small-9\">");
            Add(v);
            Add("</div>");
            return this;
        }

        public HtmlContent COL(string label, string v, int n = 0)
        {
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label);
            Add("：</div>");
            Add("<div class=\"cell small-9\">");
            Add(v);
            Add("</div>");
            return this;
        }

        public HtmlContent COL(string label, decimal v, int n = 0)
        {
            Add("<div class=\"grid-x grid-padding-x\">");
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label);
            Add("：</div>");
            Add("<div class=\"cell small-9\">");
            Add(v);
            Add("</div>");
            Add("</div>");
            return this;
        }

        public HtmlContent COL(string label, Action<HtmlContent> v)
        {
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label);
            Add("：</div>");
            Add("<div class=\"cell small-9\">");
            v(this);
            Add("</div>");
            return this;
        }

        public HtmlContent COL(string label1, string v1, string label2, string v2)
        {
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label1);
            Add("：</div>");
            Add("<div class=\"cell small-3\">");
            Add(v1);
            Add("</div>");
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label2);
            Add("：</div>");
            Add("<div class=\"cell small-3\">");
            Add(v2);
            Add("</div>");
            return this;
        }

        public HtmlContent COL(string label1, string v1, string label2, int v2)
        {
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label1);
            Add("：</div>");
            Add("<div class=\"cell small-3\">");
            Add(v1);
            Add("</div>");
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label2);
            Add("：</div>");
            Add("<div class=\"cell small-3\">");
            Add(v2);
            Add("</div>");
            return this;
        }

        public HtmlContent COL(string label1, string v1, string label2, decimal v2)
        {
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label1);
            Add("：</div>");
            Add("<div class=\"cell small-3\">");
            Add(v1);
            Add("</div>");
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label2);
            Add("：</div>");
            Add("<div class=\"cell small-3\">");
            Add(v2);
            Add("</div>");
            return this;
        }

        public HtmlContent COL(string label1, string v1, string label2, DateTime v2)
        {
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label1);
            Add("：</div>");
            Add("<div class=\"cell small-3\">");
            Add(v1);
            Add("</div>");
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label2);
            Add("：</div>");
            Add("<div class=\"cell small-3\">");
            Add(v2);
            Add("</div>");
            return this;
        }

        public HtmlContent COL_(string label)
        {
            Add("<div class=\"grid-x grid-padding-x align-middle\">");
            Add("<div class=\"cell small-3\" style=\"padding-right: 0; text-align: right\">");
            Add(label);
            Add("：</div>");
            Add("<div class=\"cell small-9\">");
            return this;
        }

        public HtmlContent COL_(int n)
        {
            Add("<div class=\"cell small-");
            Add(n);
            Add("\">");
            return this;
        }

        public HtmlContent _COL()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent CARD_()
        {
            Add("<div class=\"card\" style=\"margin: 0.25rem\">");
            return this;
        }

        public HtmlContent _CARD()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent CARDITEM(string label, string v)
        {
            Add("<div class=\"row\" style=\"margin: 0;\">");
            Add("<div class=\"small-3 columns labeldiv\">");
            Add(label);
            Add("</div>");
            Add("<div class=\"small-9 columns\">");
            Add(v);
            Add("</div>");
            Add("</div>");
            return this;
        }

        public HtmlContent FORM_(string action = null, bool post = true, bool mp = false)
        {
            Add("<form");
            if (action != null)
            {
                Add(" action=\"");
                Add(action);
                Add("\"");
            }
            if (post)
            {
                Add(" method=\"post\"");
            }
            if (mp)
            {
                Add(" enctype=\"multipart/form-data\"");
            }
            Add(">");
            Add("<div class=\"grid-x\">");
            return this;
        }

        public HtmlContent _FORM()
        {
            Add("</div>");
            Add("</form>");
            return this;
        }

        public HtmlContent FIELDSET_(string legend = null)
        {
            Add("<fieldset class=\"fieldset\">");
            if (legend != null)
            {
                Add("<legend>");
                AddEsc(legend);
                Add("</legend>");
            }
            return this;
        }

        public HtmlContent _FIELDSET()
        {
            Add("</fieldset>");
            return this;
        }

        public HtmlContent CALLOUT(string v, bool closable = false)
        {
            Add("<div class=\"callout primary\"");
            if (closable)
            {
                Add(" data-closable");
            }
            Add("><p class=\"text-center\">");
            Add(v);
            Add("</p>");
            if (closable)
            {
                Add("<button class=\"close-button\" type=\"button\" data-close><span>&times;</span></button>");
            }
            Add("</div>");
            return this;
        }

        public HtmlContent CALLOUT(Action<HtmlContent> m, bool closable)
        {
            Add("<div class=\"callout primary\"");
            if (closable)
            {
                Add(" data-closable");
            }
            Add("><p class=\"text-center\">");
            m?.Invoke(this);
            Add("</p>");
            if (closable)
            {
                Add("<button class=\"close-button\" type=\"button\" data-close><span>&times;</span></button>");
            }
            Add("</div>");
            return this;
        }

        public void TOOLBAR(Work work)
        {
            Add("<div data-sticky-container>");
            Add("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
            Add("<div class=\"top-bar\">");

            Add("<div class=\"top-bar-left\">");
            TRIGGERS(work, null);
            Add("</div>");

            Add("<div class=\"top-bar-right\">");
            Add("<a class=\"primary\" href=\"javascript: location.reload(false);\">");
            Add("<i class=\"fi-refresh\" style=\"font-size: 1.75rem; line-height: 2rem\"></i>");
            Add("</a>");
            Add("</div>");

            Add("</div>");
            Add("</div>");
            Add("</div>");
        }

        public void PAGENATE(int count)
        {
            // pagination
            ActionInfo ai = ac.Doer;
            if (ai.HasSubscript)
            {
                Add("<ul class=\"pagination text-center\" role=\"navigation\">");
                int subscpt = ac.Subscript;
                for (int i = 0; i <= subscpt; i++)
                {
                    if (subscpt == i)
                    {
                        Add("<li class=\"current\">");
                        Add(i + 1);
                        Add("</li>");
                    }
                    else
                    {
                        Add("<li><a href=\"");
                        Add(ai.Name);
                        Add('-');
                        Add(i);
                        Add(ac.QueryString);
                        Add("\">");
                        Add(i + 1);
                        Add("</a></li>");
                    }
                }
                if (count == ai.Limit)
                {
                    Add("<li class=\"pagination-next\"><a href=\"");
                    Add(ai.Name);
                    Add('-');
                    Add(subscpt + 1);
                    Add(ac.QueryString);
                    Add("\">");
                    Add(subscpt + 2);
                    Add("</a></li>");
                }
                Add("</ul>");
            }
        }

        public HtmlContent TableView(string name, IDataInput inp, Action<IDataInput, HtmlContent, char> putter)
        {
            Add("<table class=\"unstriped\">");

            Add("<thead>");
            Add("<tr>");
            if (name != null)
            {
                Add("<th></th>");
            }
            putter(inp, this, 'L'); // putting value
            Add("</tr>");
            Add("</thead>");

            Add("<tbody>");
            while (inp.Next())
            {
                Add("<tr>");
                putter(inp, this, 'B'); // putting label
                Add("</tr>");
            }
            Add("</tbody>");
            Add("</table>");
            return this;
        }

        public HtmlContent TableView<D>(D[] arr, Action<HtmlContent> hd, Action<HtmlContent, D> row) where D : IData
        {
            Work work = ac.Work;
            Work varwork = work.varwork;

            Add("<form id=\"viewform\">");
            TOOLBAR(work);
            Add("</form>");

            Add("<table class=\"scroll unstriped\">");
            ActionInfo[] ais = varwork?.UiActions;

            Add("<thead>");
            Add("<tr>");
            // for checkboxes
            Add("<th></th>");
            hd(this);
            if (ais != null)
            {
                Add("<th></th>"); // for triggers
            }
            Add("</tr>");
            Add("</thead>");

            if (arr != null) // tbody if having data objects
            {
                Add("<tbody>");
                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    Add("<tr>");
                    // checkbox
                    Add("<td>");
                    Add("<input name=\"key\" type=\"checkbox\" form=\"viewform\"  value=\"");
                    varwork?.OutputVarKey(obj, this);
                    Add("\"></td>");
                    row(this, obj);
                    if (ais != null) // triggers
                    {
                        Add("<td style=\"width: 1px; white-space: nowrap;\">");
                        Add("<form>");
                        TRIGGERS(varwork, obj);
                        Add("</form>");
                        Add("</td>");
                    }
                    Add("</tr>");
                }
                Add("</tbody>");
            }
            Add("</table>");

            // pagination controls if any
            PAGENATE(arr?.Length ?? 0);

            return this;
        }

        public HtmlContent GridView(IDataInput inp, Action<IDataInput, HtmlContent> pipe)
        {
            if (inp != null)
            {
                Add("<div class=\"expanded row\">");
                while (inp.Next())
                {
                    Add("<div class=\"row small-up-1 medium-up-2 large-up-3 xlarge-up-4\">");
                    pipe(inp, this);
                    Add("</div>");
                }
                Add("</div>");
            }
            else
            {
                Add("<div class=\"row\">");
                Add("<span>没有记录</span>");
                Add("</div>");
            }
            --level;
            return this;
        }

        public HtmlContent GridView<D>(D[] arr, Action<HtmlContent, D> cell) where D : IData
        {
            Work work = ac.Work;
            Work varwork = work.varwork;

            Add("<form id=\"viewform\">");
            TOOLBAR(work);
            Add("</form>");

            if (arr != null) // render grid cells
            {
                Add("<div class=\"grid-x small-up-1 medium-up-2 large-up-3\">");
                for (int i = 0; i < arr.Length; i++)
                {
                    Add("<div class=\"cell\" style=\"padding: 0.5rem\">");
                    Add("<form>");
                    D obj = arr[i];

                    Add("<div class=\"grid-x\" style=\"border: 1px solid #e6e6e6; padding: 0.5rem\">");
                    Add("<div class=\"cell\">");
                    Add("<input name=\"key\" type=\"checkbox\" form=\"viewform\" value=\"");
                    varwork?.OutputVarKey(obj, this);
                    Add("\">");
                    Add("</div>");

                    cell(this, obj);

                    // output var triggers
                    Add("<div class=\"cell\" style=\"text-align: right\">");
                    TRIGGERS(varwork, obj);
                    Add("</div>");

                    Add("</div>");
                    Add("</form>");
                    Add("</div>");
                }
                Add("</div>");
            }
            else // empty
            {
                Add("<div class=\"grid-x\">");
                Add("</div>");
            }

            // pagination if any
            PAGENATE(arr?.Length ?? 0);

            return this;
        }

        public HtmlContent ListView<D>(D[] arr, short proj = 0x00ff) where D : IData
        {
            Add("<form id=\"listform\">");

            if (arr != null)
            {
                Add("<ul>");
                for (int i = 0; i < arr.Length; i++)
                {
                    Add("<li>");
                    arr[i].Write(this, proj);
                    Add("</li>");
                }
                Add("</ul>");
                --level;
            }

            if (ac != null)
            {
                // pagination controls if any
                PAGENATE(arr?.Length ?? 0);
                Add("</form>");
            }
            return this;
        }

        public HtmlContent TRIGGERS(Work work, IData obj)
        {
            var ais = work.UiActions;
            if (ais == null)
            {
                return this;
            }
            for (int i = 0; i < ais.Length; i++)
            {
                ActionInfo ai = ais[i];
                // access check if neccessary
                if (ac != null && !ai.DoAuthorize(ac)) continue;

                UiAttribute ui = ai.Ui;

                // check state covering
                bool enabled = true;
                var stateobj = obj as IStatable;
                if (stateobj != null)
                {
                    enabled = ui.Covers(stateobj.GetState());
                }

                if (ui.IsA)
                {
                    Add("<a class=\"button hollow");
//                    Add(ac?.Doer == ai ? " hollow" : " clear");
                    Add(" primary\" href=\"");
                    if (obj != null)
                    {
                        ai.Work.OutputVarKey(obj, this);
                        Add('/');
                    }
                    Add(ai.RPath);
                    Add("\"");
                    if (ui.HasPrompt)
                    {
                        Add(" onclick=\"return dialog(this,2,1,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasShow)
                    {
                        Add(" onclick=\"return dialog(this,4,2,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasOpen)
                    {
                        Add(" onclick=\"return dialog(this,8,3,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasScript)
                    {
                        Add(" onclick=\"if(!confirm('");
                        Add(ui.Tip ?? ui.Label);
                        Add("')) return false;");
                        Add(ai.Name);
                        Add("(this);return false;\"");
                    }
                    else if (ui.HasCrop)
                    {
                        Add(" onclick=\"return crop(this,");
                        Add(ui.X);
                        Add(',');
                        Add(ui.Y);
                        Add(',');
                        Add(ui.Circle);
                        Add(",'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    if (!enabled)
                    {
                        Add(" disabled onclick=\"return false;\"");
                    }
                    Add(">");
                    Add(ai.Label);
                    Add("</a>");
                }
                else if (ui.IsButton)
                {
                    Add("<button class=\"button primary");
                    if (!ui.Em) Add(" hollow");
                    Add("\" name=\"");
                    Add(ai.Name);
                    Add("\" formaction=\"");
                    if (obj != null)
                    {
                        ai.Work.OutputVarKey(obj, this);
                        Add('/');
                    }
                    Add(ai.Name);
                    Add("\" formmethod=\"post\"");
                    if (!enabled)
                    {
                        Add(" disabled");
                    }
                    else if (ui.HasConfirm)
                    {
                        Add(" onclick=\"return confirm('");
                        Add(ui.Tip ?? ui.Label);
                        Add("?');\"");
                    }
                    else if (ui.HasPrompt)
                    {
                        Add(" onclick=\"return dialog(this,2,1,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasShow)
                    {
                        Add(" onclick=\"return dialog(this,4,2,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasOpen)
                    {
                        Add(" onclick=\"return dialog(this,8,3,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    Add(">");
                    Add(ai.Label);
                    Add("</button>");
                }
            }
            return this;
        }

        public HtmlContent BUTTON(string value, bool post = true)
        {
            Add("<button class=\"button primary hollow\" formmethod=\"");
            Add(post ? "post" : "get");
            Add("\">");
            AddEsc(value);
            Add("</button>");
            return this;
        }

        public HtmlContent BUTTON(string name, int subcmd, string value, bool post = true)
        {
            Add("<button class=\"button primary hollow\" formmethod=\"");
            Add(post ? "post" : "get");
            Add("\" formaction=\"");
            Add(name);
            Add('-');
            Add(subcmd);
            Add("\">");
            AddEsc(value);
            Add("</button>");
            return this;
        }


        public HtmlContent HIDDEN(string name, string value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            Add("\">");
            return this;
        }

        public HtmlContent HIDDEN(string name, int value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add("\">");
            return this;
        }

        public HtmlContent HIDDEN(string name, decimal value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add("\">");
            return this;
        }

        public HtmlContent TEXT(string name, string v, string label = null, string help = null, string pattern = null, sbyte max = 0, sbyte min = 0, IOptable<string> opt = null, bool @readonly = false, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");
                Add(" size=\"");
                Add(max);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");
            Add("</label>");

            GridEnd(endg);
            return this;
        }

        public HtmlContent SEARCH(string name, string v, string label = null, string help = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            if (label != null)
            {
                Add("<label>");
                Add(label);
            }
            Add("<input type=\"search\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");
                Add(" size=\"");
                Add(max);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            Add(">");
            if (label != null)
            {
                Add("</label>");
            }

            GridEnd(endg);
            return this;
        }

        public HtmlContent PASSWORD(string name, string v, string label = null, string help = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"password\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (help != null)
            {
                Add(" Help=\"");
                Add(help);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");
                Add(" size=\"");
                Add(max);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");
            Add("</label>");

            GridEnd(endg);
            return this;
        }

        public HtmlContent DATE(string name, DateTime v, string label = null, DateTime max = default(DateTime), DateTime min = default(DateTime), bool @readonly = false, bool required = false, int step = 0, int n = 0)
        {
            int endg = GridStart(n);

            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"date\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (max != default(DateTime))
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != default(DateTime))
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (step != 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }
            Add(">");
            Add("</label>");

            GridEnd(endg);
            return this;
        }

        public HtmlContent TIME()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent NUMBER(string name, short v, string label = null, string tip = null, short max = 0, short min = 0, short step = 0, bool opt = false, bool @readonly = false, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            if (label != null)
            {
                Add("<label>");
                AddLabel(label, name);
            }

            bool group = step > 0; // input group with up up and down

            if (group)
            {
                Add("<div class=\"input-group\">");
                Add("<input type=\"button\" class=\"input-group-label\" onclick=\"this.form['");
                Add(name);
                Add("'].stepDown()\" value=\"-\">");
            }

            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (group)
            {
                Add(" class=\"input-group-field\"");
            }

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (step != 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }
            if (opt)
            {
                Add("<input type=\"button\" value=\"...\" onclick=\"dialog(this, 1, 1)\"> ");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");

            if (group)
            {
                Add("<input type=\"button\" class=\"input-group-label\" onclick=\"this.form['");
                Add(name);
                Add("'].stepUp()\" value=\"+\">");
                Add("</div>");
            }

            if (label != null)
            {
                Add("</label>");
            }

            GridEnd(endg);
            return this;
        }

        public HtmlContent NUMBER(string name, int v, string label = null, string tip = null, int max = 0, int min = 0, int step = 0, bool opt = false, bool @readonly = false, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (step != 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }
            if (opt)
            {
                Add("<input type=\"button\" value=\"...\" onclick=\"dialog(this, 1, 1)\"> ");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            Add("</label>");

            GridEnd(endg);
            return this;
        }

        public HtmlContent NUMBER(string name, long v, string label = null, string tip = null, long max = 0, long min = 0, long step = 0, bool opt = false, bool @readonly = false, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (step != 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }
            if (opt)
            {
                Add("<input type=\"button\" value=\"...\" onclick=\"dialog(this, 1, 1)\"> ");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            Add("</label>");

            GridEnd(endg);
            return this;
        }

        public HtmlContent NUMBER(string name, decimal v, string label = null, string tip = null, decimal max = 0, decimal min = 0, decimal step = 0, bool @readonly = false, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }

            Add(" step=\"");
            if (step > 0) Add(step);
            else Add("any");
            Add("\"");

            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            Add("</label>");

            GridEnd(endg);
            return this;
        }

        public HtmlContent NUMBER(string name, double v, string label = null, string tip = null, double max = 0, double min = 0, double step = 0, bool @readonly = false, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }

            Add(" step=\"");
            if (step > 0) Add(step);
            else Add("any");
            Add("\"");

            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            Add("</label>");

            GridEnd(endg);
            return this;
        }

        public HtmlContent RANGE()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent COLOR()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent CHECKBOXES(string name, IDataInput inp, Action<IDataInput, HtmlContent, char> putter)
        {
            while (inp.Next())
            {
                Add("<label>");
                Add("<input type=\"checkbox\" name=\"");
                Add(name);
                Add("\" value=\"");
                putter(inp, this, 'V'); // putting value
                Add("\">");
                putter(inp, this, 'L'); // putting label
                Add("</label>");
            }
            return this;
        }

        public HtmlContent CHECKBOX(string name, bool v, string label = null, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            if (label != null)
            {
                Add("<label>");
            }
            Add("<input type=\"checkbox\" name=\"");
            Add(name);
            Add("\"");
            if (v) Add(" checked");
            if (required) Add(" required");
            Add(">");
            if (label != null)
            {
                Add(label);
                Add(" </label>");
            }

            GridEnd(endg);
            return this;
        }

        public HtmlContent CHECKBOXGROUP(string name, string[] v, string[] opts, string legend = null)
        {
            if (legend != null)
            {
                Add("<fieldset>");
                Add("<legend>");
                Add(legend);
                Add("</legend>");
            }

            for (int i = 0; i < opts.Length; i++)
            {
                var item = opts[i];
                Add(" <label>");
                Add("<input type=\"checkbox\" name=\"");
                Add(name);
                Add("\"");
                if (v != null && v.Contains(item))
                {
                    Add(" checked");
                }
                Add(">");
                Add(item);
                Add(" </label>");
            }

            if (legend != null)
            {
                Add("</fieldset>");
            }
            return this;
        }

        public HtmlContent RADIO(string name, int value, bool @checked, string label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (@checked)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(label);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, long value, bool check, string label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (check)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(label);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, string value, bool check, string label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (check)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(label);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, Action<HtmlContent> value, bool @checked, Action<HtmlContent> label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            value(this);
            if (@checked)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            label(this);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, string v1, string v2, string v3, bool @checked, string l1, string l2, string l3)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v1);
            if (v2 != null)
            {
                Add('~');
                Add(v2);
            }
            if (v3 != null)
            {
                Add('~');
                Add(v3);
            }
            if (@checked)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(l1);
            if (l2 != null)
            {
                Add(' ');
                Add(l2);
            }
            if (l3 != null)
            {
                Add(' ');
                Add(l3);
            }
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, long v1, string v2, bool @checked, long l1, string l2, string l3 = null)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v1);
            if (v2 != null)
            {
                Add('-');
                Add(v2);
            }
            if (@checked)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(l1);
            Add(' ');
            Add(l2);
            if (l3 != null)
            {
                Add(' ');
                Add(l3);
            }
            Add("</label>");
            return this;
        }

        public HtmlContent RADIOS(string name, short v, IOptable<short> opt = null, string label = null, bool required = false)
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(label, name);
            Add("</legend>");

            opt?.ForEach((key, item) =>
            {
                Add("<label>");
                Add("<input type=\"radio\" name=\"");
                Add(name);

                Add("\" id=\"");
                Add(name);
                Add(key);
                Add("\"");

                Add("\" value=\"");
                Add(key);
                Add("\"");

                if (key.Equals(v)) Add(" checked");
                if (required) Add(" required");
                Add(">");
                Add("</label>");

                Add(key);
//                Add("<label for=\"");
//                Add(name);
//                Add(key);
//                Add("\">");
//                Add(item.ToString());
//                Add("</label>");
            });

            Add("</fieldset>");
            return this;
        }

        public HtmlContent RADIOS(string name, string v, IOptable<string> opt = null, string label = null, bool required = false)
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(label, name);
            Add("</legend>");

            opt?.ForEach((key, item) =>
            {
                Add("<input type=\"radio\" name=\"");
                Add(name);

                Add("\" id=\"");
                Add(name);
                Add(key);
                Add("\"");

                Add("\" value=\"");
                Add(key);
                Add("\"");

                if (key.Equals(v)) Add(" checked");
                if (required) Add(" required");
                Add(">");

                Add("<label for=\"");
                Add(name);
                Add(key);
                Add("\">");
                Add(item.ToString());
                Add("</label>");
            });

            Add("</fieldset>");
            return this;
        }

        public HtmlContent RADIOGROUP(string name, string v, string[] opt, string legend = null, bool required = false)
        {
            if (legend != null)
            {
                Add("<fieldset>");
                Add("<legend>");
                Add(legend);
                Add("</legend>");
            }

            for (int i = 0; i < opt.Length; i++)
            {
                var item = opt[i];
                Add("<label>");
                Add("<input type=\"radio\" name=\"");
                Add(name);
                Add("\" value=\"");
                Add(item);
                Add("\"");

                if (item.Equals(v)) Add(" checked");
                if (required) Add(" required");
                Add(">");

                Add(item);
                Add("</label>");
            }
            if (legend != null)
            {
                Add("</fieldset>");
            }
            return this;
        }

        public HtmlContent TEXTAREA(string name, string v, string label = null, string help = null, short max = 0, short min = 0, bool @readonly = false, bool required = false, int n = 0)
        {
            int endg = GridStart(n);

            Add("<label>");
            AddLabel(label, name);
            Add("<textarea name=\"");
            Add(name);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");

                Add(" rows=\"");
                Add(max < 200 ? 3 : max < 400 ? 4 : 5);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            AddEsc(v);
            Add("</textarea>");
            Add("</label>");

            GridEnd(endg);
            return this;
        }

        public HtmlContent BUTTON(ActionInfo ai)
        {
            Add("<button class=\"button primary\" style=\"margin-right: 5px; border-radius: 15%\"");
            Add(" formaction=\"");
            Add(ai.Name);
            Add("\" formmethod=\"post\"");

            UiAttribute ui = ai.Ui;

            Modal mode = ui.Modal;
            if (mode > 0)
            {
                Add(" onclick=\"dialog(this,");
                Add((int) mode);
                Add("); return false;\"");
            }
            Add(">");
            AddLabel(ui.Label, ai.Name);

            Add("</button>");
            return this;
        }

        int gridx;

        int GridStart(int n)
        {
            if (gridx > 0)
            {
                if (n == 0 || n > gridx) n = gridx;
                Add("<div class=\"small-");
                Add(n);
                Add(" cell\">");
                gridx -= n;
                return gridx > 0 ? 2 : 1;
            }
            if (n > 0)
            {
                Add("<div class=\"grid-x grid-margin-x\">");
                Add("<div class=\"small-");
                Add(n);
                Add(" cell\">");
                gridx = 12 - n;
                return gridx > 0 ? 2 : 1;
            }
            return 0;
        }

        void GridEnd(int c)
        {
            if (c == 1)
            {
                Add("</div>");
            }
            else if (c == 2)
            {
                Add("</div></div>");
            }
        }

        public HtmlContent SELECT(string name, short v, IOptable<short> opt, string label = null, bool multiple = false, bool required = false, int size = 0, int n = 0)
        {
            int endg = GridStart(n);

            Add("<label>");
            AddLabel(label, name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (multiple) Add(" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            Add(">");

            opt?.ForEach((key, item) =>
            {
                Add("<option value=\"");
                Add(key);
                Add("\"");
                if (key == v) Add(" selected");
                Add(">");
                Add(item.ToString());
                Add("</option>");
            });

            Add("</select>");
            Add("</label>");

            GridEnd(endg);
            return this;
        }

        public HtmlContent SELECT(string name, string v, IOptable<string> opt, string label = null, bool multiple = false, bool required = false, sbyte size = 0, bool refresh = false, int n = 0)
        {
            int endg = GridStart(n);

            if (label != null)
            {
                Add("<label>");
                Add(label);
            }
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (multiple) Add(" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + $(this.form).serialize();\"");
            }
            Add(">");

            opt?.ForEach((key, text) =>
            {
                Add("<option value=\"");
                Add(key);
                Add("\"");
                if (key == v) Add(" selected");
                Add(">");
                Add(opt.Obtain(key));
                Add("</option>");
            });
            Add("</select>");
            if (label != null)
            {
                Add("</label>");
            }

            GridEnd(endg);
            return this;
        }

        public HtmlContent SELECT(string name, string v, string[] opt, string label = null, bool multiple = false, bool required = false, sbyte size = 0, bool refresh = false, int n = 0)
        {
            int endg = GridStart(n);

            if (label != null)
            {
                Add("<label>");
                Add(label);
            }
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (multiple) Add(" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + $(this.form).serialize();\"");
            }
            Add(">");

            if (opt != null)
            {
                for (int i = 0; i < opt.Length; i++)
                {
                    string key = opt[i];
                    Add("<option value=\"");
                    Add(key);
                    Add("\"");
                    if (key == v) Add(" selected");
                    Add(">");

                    Add(key);
                    Add("</option>");
                }
            }
            Add("</select>");
            if (label != null)
            {
                Add("</label>");
            }

            GridEnd(endg);
            return this;
        }

        public HtmlContent DATALIST()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent PROGRES()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent OUTPUT(string name)
        {
            Add("<output name=\"");
            Add(name);
            Add("\"></output>");
            return this;
        }

        public HtmlContent METER()
        {
            T("</tbody>");
            return this;
        }


        //
        // ISINK
        //
        public HtmlContent JSON(IData obj, short proj = 0x00ff)
        {
            kind = JS;
            Put(null, obj, proj);
            return this;
        }

        public HtmlContent JSON<D>(D[] arr, short proj = 0x00ff) where D : IData
        {
            kind = JS;
            Put(null, arr, proj);
            return this;
        }

        public HtmlContent JSON<K, D>(Map<K, D> map, short proj = 0x00ff) where D : IData
        {
            kind = JS;
            Put(null, map, proj);
            return this;
        }


        public HtmlContent PutNull(string name)
        {
            return this;
        }

        public HtmlContent Put(string name, JNumber v)
        {
            return this;
        }

        public HtmlContent Put(string name, IDataInput v)
        {
            return this;
        }

        public HtmlContent Put(string name, bool v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    Add(v ? "true" : "false");
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, short v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    Add(v);
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, int v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    Add(v);
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, long v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    Add(v);
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, double v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    Add(v);
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, decimal v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    Add(v);
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, DateTime v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    Add('"');
                    Add(v);
                    Add('"');
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, string v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    if (v == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        Add('"');
                        AddJsonEsc(v);
                        Add('"');
                    }
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, ArraySegment<byte> v)
        {
            switch (kind)
            {
                case JS:
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, short[] v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    if (v == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        Add('[');
                        for (int i = 0; i < v.Length; i++)
                        {
                            if (i > 0) Add(',');
                            Add(v[i]);
                        }
                        Add(']');
                    }
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, int[] v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    if (v == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        Add('[');
                        for (int i = 0; i < v.Length; i++)
                        {
                            if (i > 0) Add(',');
                            Add(v[i]);
                        }
                        Add(']');
                    }
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, long[] v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    if (v == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        Add('[');
                        for (int i = 0; i < v.Length; i++)
                        {
                            if (i > 0) Add(',');
                            Add(v[i]);
                        }
                        Add(']');
                    }
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, string[] v)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    if (v == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        Add('[');
                        for (int i = 0; i < v.Length; i++)
                        {
                            if (i > 0) Add(',');
                            string str = v[i];
                            if (str == null)
                            {
                                Add("null");
                            }
                            else
                            {
                                Add('"');
                                AddJsonEsc(str);
                                Add('"');
                            }
                        }
                        Add(']');
                    }
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, Map<string, string> v)
        {
            switch (kind)
            {
                case JS:
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, IData v, short proj = 0x00ff)
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    if (v == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        counts[++level] = 0; // enter
                        Add('{');

                        // put shard property if any
                        string shard = (v as IShardable)?.Shard;
                        if (shard != null)
                        {
                            Put("#", shard);
                        }

                        v.Write(this, proj);
                        Add('}');
                        level--; // exit
                    }
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put<D>(string name, D[] v, short proj = 0x00ff) where D : IData
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    if (v == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        counts[++level] = 0; // enter
                        Add('[');
                        for (int i = 0; i < v.Length; i++)
                        {
                            Put(null, v[i], proj);
                        }
                        Add(']');
                        level--; // exit
                    }
                    break;
                case UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put<K, D>(string name, Map<K, D> v, short proj = 0x00ff) where D : IData
        {
            switch (kind)
            {
                case JS:
                    if (counts[level]++ > 0) Add(',');
                    if (name != null)
                    {
                        Add('"');
                        Add(name);
                        Add('"');
                        Add(':');
                    }
                    if (v == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        counts[++level] = 0; // enter
                        Add('[');
                        foreach (var pair in v)
                        {
                            Put(null, pair.Value, proj);
                        }
                        Add(']');
                        level--; // exit
                    }
                    break;
                case UL:
                    break;
            }
            return this;
        }
    }
}