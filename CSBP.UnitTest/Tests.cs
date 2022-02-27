using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CSBP.Apis.Models.Extension;
using CSBP.Base;
using NUnit.Framework;

namespace CSBP.UnitTest
{
  public class Tests
  {
    const string pfadmono = "/home/wolfgang/mono";
    const string pfad = "/home/wolfgang/cs";
    const string formpath = "/home/wolfgang/git/jhh6/Jhh/src/main/resources/dialog";

    /// <summary>
    /// Starting tests manually.
    /// </summary>
    /// <param name="args">Parameters are ignored.</param>
    static void Main(string[] args)
    {
      Debug.Print("CSBP.UnitTest gestartet.");
      var t = new Tests();
      t.MachNichts();
      // t.GenerateForm();
      t.GenerateResxDesigner();
      // t.Tls();
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void MachNichts()
    {
      Assert.AreEqual(0, Functions.MachNichts());
    }

    /// <summary>
    /// XML-Dateien der Tabellen zusammenkopieren.
    /// </summary>
    ////[Test()]
    public void MergeXml()
    {
      Assert.IsNull(null);
      var l = Directory.GetFiles(pfadmono, "*.xml");
      Assert.IsTrue(l.Length > 0);
      var g = XDocument.Load(l[0]);
      var node = g.Root;
      for (var i = 1; i < l.Length; i++)
      {
        var xml = XDocument.Load(l[i]);
        node.Add(xml.Descendants("table").First());
      }
      g.Save(Path.Combine(pfadmono, "#CSBP.xml.txt"));
    }

    private string GetCsType(string dbtype, bool nullable)
    {
      switch (dbtype)
      {
        case "INTEGER": return "int" + (nullable ? "?" : "");
        case "VARCHAR": return "string";
        case "DATE": return "DateTime" + (nullable ? "?" : "");
        case "TIMESTAMP": return "DateTime" + (nullable ? "?" : "");
        case "DECIMAL(21,4)": return "decimal" + (nullable ? "?" : "");
        case "BOOLEAN": return "bool" + (nullable ? "?" : "");
        case "BLOB": return "byte[]";
        default: return dbtype;
      }
    }

    /// <summary>
    /// Generieren der Entity-Klassen und der CsbpContext-Klasse.
    /// </summary>
    [Test]
    public void GenerierenModelCs()
    {
      var g = XDocument.Load(Path.Combine(pfad, "csbp/CSBP/Resources/Tables.xml"));
      var tables = g.Descendants("table");
      var sets = new StringBuilder();
      var keys = new StringBuilder();
      foreach (var t in tables)
      {
        var tabelle = t.Attribute("name").Value;
        if (tabelle.StartsWith("HP_", StringComparison.CurrentCulture)
            || tabelle.StartsWith("MO_", StringComparison.CurrentCulture)
            //|| tabelle.StartsWith("SO_", StringComparison.CurrentCulture)
            || tabelle.StartsWith("VM_", StringComparison.CurrentCulture))
          continue;
        var tab = Functions.TabName(tabelle);

        if (!tabelle.StartsWith("SO_", StringComparison.CurrentCulture))
        {
          if (sets.Length > 0)
            sets.Append(Environment.NewLine).Append(Environment.NewLine);
          sets.Append($@"    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle {tabelle}.</summary>
    public DbSet<{tab}> {tabelle} {{ get; set; }}");
        }

        var props = new StringBuilder();
        var columns = t.Descendants("column");
        foreach (var c in columns)
        {
          var name = c.Attribute("name").Value;
          var type = c.Attribute("type").Value;
          var length = c.Attribute("length")?.Value;
          var nullable = bool.Parse(c.Attribute("nullable").Value);
          var extension = bool.Parse(c.Attribute("extension")?.Value ?? "false");
          if (props.Length > 0)
            props.Append(Environment.NewLine).Append(Environment.NewLine);
          if (extension)
            props.Append($@"    /// <summary>Holt oder setzt den Wert der Spalte {name}.</summary>
    public {GetCsType(type, nullable)} {name} {{
      get {{
        return GetExtension();
      }}
      set {{
        SetExtension(value);
      }}
    }}");
          else
            props.Append($@"    /// <summary>Holt oder setzt den Wert der Spalte {name}.</summary>
    public {GetCsType(type, nullable)} {name} {{ get; set; }}");
        }
        var keycolumns = t.Descendants("keycolumn");
        var pks = string.Join(", ", keycolumns.Select(a => "a." + a.Attribute("name").Value));
        if (keycolumns.Count() > 1)
          pks = "new { " + pks + " }";
        if (!tabelle.StartsWith("SO_", StringComparison.CurrentCulture))
        {
          if (keys.Length > 0)
            keys.Append(Environment.NewLine);
          keys.Append($@"      modelBuilder.Entity<{tab}>().HasKey(a => {pks});");
        }
        var model = $@"// <copyright file=""{tab}.cs"" company=""cwkuehl.de"">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle {tabelle}.
  /// </summary>
  [Serializable]
  [Table(""{tabelle}"")]
  public partial class {tab} : ModelBase
  {{
    /// <summary>Initialisiert eine neue Instanz der <see cref=""{tab}""/> Klasse.</summary>
    public {tab}()
    {{
      Functions.MachNichts();
    }}

{props.ToString()}
  }}
}}
";
        File.WriteAllText(Path.Combine(pfad, "csbp/CSBP/Apis/Models", tab + ".cs"), model);
      }
      var context = $@"// <copyright file=""CsbpContext2.cs"" company=""cwkuehl.de"">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories.Base
{{
  using CSBP.Apis.Models;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Generierter Teil des Datenbank-Context.
  /// </summary>
  public partial class CsbpContext : DbContext
  {{
{sets.ToString()}

    /// <summary>
    /// On the model creating generated.
    /// </summary>
    /// <param name=""modelBuilder"">Model builder.</param>
    void OnModelCreatingGenerated(ModelBuilder modelBuilder)
    {{
{keys.ToString()}
    }}
  }}
}}
";
      File.WriteAllText(Path.Combine(pfad, "csbp/CSBP/Services/Repositories/Base", "CsbpContext2.cs"), context);
    }

    /// <summary>
    /// Generieren der Repository-Klassen.
    /// </summary>
    [Test]
    public void GenerierenReps()
    {
      var g = XDocument.Load(Path.Combine(pfad, "csbp/CSBP/Resources/Tables.xml"));
      var tables = g.Descendants("table");
      var sets = new StringBuilder();
      var keys = new StringBuilder();

      foreach (var t in tables)
      {
        var tabelle = t.Attribute("name").Value;
        if (tabelle.StartsWith("HP_", StringComparison.CurrentCulture)
            || tabelle.StartsWith("MO_", StringComparison.CurrentCulture)
            || tabelle.StartsWith("SO_", StringComparison.CurrentCulture)
            || tabelle.StartsWith("VM_", StringComparison.CurrentCulture))
          continue;
        //if (!tabelle.StartsWith("FZ_Fahrrads", StringComparison.CurrentCulture))
        //  continue;
        var tab = Functions.TabName(tabelle);

        var kparam = new StringBuilder(); // Key parameters
        var kparam2 = new StringBuilder(); // Key parameters without types
        var gparam2 = new StringBuilder(); // All parameters
        var gwhere = new StringBuilder();
        var gwhere2 = new StringBuilder();
        var lparam = new StringBuilder();
        var lwhere = new StringBuilder();
        var getvalue = new StringBuilder();
        var keycolumns = t.Descendants("keycolumn");
        var columns = t.Descendants("column");
        var ps = columns.Select(a => new
        {
          name = a.Attribute("name").Value,
          type = a.Attribute("type").Value,
          length = a.Attribute("length")?.Value,
          nullable = bool.Parse(a.Attribute("nullable").Value),
          key = keycolumns.Any(b => b.Attribute("name").Value == a.Attribute("name").Value),
          cstype = GetCsType(a.Attribute("type").Value, bool.Parse(a.Attribute("nullable").Value))
        });
        var autouid = ps.Any(a => a.name == "Uid")
            ? @"
      e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;" : string.Empty;
        foreach (var p in ps)
        {
          var vname = p.name.Replace("_", string.Empty).ToLower();
          gparam2.Append($", {p.cstype} {vname}");
          if (p.name.StartsWith("Angelegt_", StringComparison.Ordinal)
              || p.name.StartsWith("Geaendert_", StringComparison.Ordinal)
              || p.name.StartsWith("Replikation_Uid", StringComparison.Ordinal))
            gparam2.Append(" = null");
          if (p.name == "Mandant_Nr")
          {
            lparam.Append($", {p.cstype} {vname}");
            if (lwhere.Length <= 0)
              lwhere.Append(".Where(a => ");
            lwhere.Append($"a.{p.name} == {vname})");
          }
          if (p.key)
          {
            kparam.Append($", {p.cstype} {vname}");
            kparam2.Append($", {vname}");
            if (gwhere.Length > 0)
              gwhere.Append(" && ");
            //if (p.cstype == "DateTime")
            //  gwhere.Append($"a.{p.name} >= e.{p.name} && a.{p.name} < e.{p.name}.AddSeconds(1)");
            //else
            gwhere.Append($"a.{p.name} == e.{p.name}");
            if (gwhere2.Length > 0)
              gwhere2.Append(" && ");
            //if (p.cstype == "DateTime")
            //  gwhere2.Append($"a.{p.name} >= {vname} && a.{p.name} < {vname}.AddSeconds(1)");
            //else
            gwhere2.Append($"a.{p.name} == {vname}");
          }
        }
        getvalue.Append(ps.Any(a => a.name == "Uid")
            ? $@"var a = string.IsNullOrEmpty(uid) ? null : Get(daten{kparam2.ToString()});
" : $@"var a = Get(daten{kparam2.ToString()});
").Append($@"      var e = a ?? new {tab}();
");
        foreach (var p in ps)
        {
          var vname = p.name.Replace("_", string.Empty).ToLower();
          if (p.name == "Uid")
          {
            getvalue.Append($@"      e.{p.name} = string.IsNullOrEmpty({vname}) ? Functions.GetUid() : {vname};
");
          }
          else if (!(p.name.StartsWith("Angelegt_", StringComparison.Ordinal)
            || p.name.StartsWith("Geaendert_", StringComparison.Ordinal)
            || p.name.StartsWith("Replikation_Uid", StringComparison.Ordinal)))
          {
            getvalue.Append($@"      e.{p.name} = {vname};
");
          }
        }

        var rep = $"{tab}Rep";
        var s = $@"// <copyright file=""{rep}.cs"" company=""cwkuehl.de"">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Repositories.Base;

  /// <summary>
  /// Generierte Basis-Klasse für {tabelle}-Repository.
  /// </summary>
  public partial class {rep} : RepositoryBase
  {{
    public {tab} Get(ServiceDaten daten, {tab} e)
    {{
      var db = GetDb(daten);
      var b = db.{tabelle}.FirstOrDefault(a => {gwhere.ToString()});
      return b;
    }}

    public {tab} Get(ServiceDaten daten{kparam.ToString()}, bool detached = false)
    {{
      var db = GetDb(daten);
      var b = db.{tabelle}.FirstOrDefault(a => {gwhere2.ToString()});
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }}

    public List<{tab}> GetList(ServiceDaten daten{lparam.ToString()})
    {{
      var db = GetDb(daten);
      var l = db.{tabelle}{lwhere.ToString()};
      return l.ToList();
    }}

    public void Insert(ServiceDaten daten, {tab} e)
    {{
      var db = GetDb(daten);{autouid}
      MachAngelegt(e, daten);
      db.{tabelle}.Add(e);
    }}

    public void Update(ServiceDaten daten, {tab} e)
    {{
      var db = GetDb(daten);
      var a = Get(daten, e);
      db.Entry(a).CurrentValues.SetValues(e);
      if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {{
        MachGeaendert(a, daten);
        db.{tabelle}.Update(a);
      }}
    }}

    public {tab} Save(ServiceDaten daten{gparam2.ToString()})
    {{
      var db = GetDb(daten);
      {getvalue}      if (a == null)
      {{
        MachAngelegt(e, daten, angelegtam, angelegtvon);
        if (!string.IsNullOrEmpty(geaendertvon))
          MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.{tabelle}.Add(e);
      }}
      else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {{
        if (!string.IsNullOrEmpty(angelegtvon))
          MachAngelegt(e, daten, angelegtam, angelegtvon);
        MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.{tabelle}.Update(e);
      }}
      return e;
    }}

    public void Delete(ServiceDaten daten, {tab} e)
    {{
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.{tabelle}.Remove(a);
    }}
  }}
}}
";
        File.WriteAllText(Path.Combine(pfad, "csbp/CSBP/Services/Repositories/Gen", $"{rep}.cs"), s);

        s = $@"// <copyright file=""{rep}.cs"" company=""cwkuehl.de"">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{{
  using System;

  /// <summary>
  /// Klasse für {tabelle}-Repository.
  /// </summary>
  public partial class {rep}
  {{
  }}
}}
";
        var datei = Path.Combine(pfad, "csbp/CSBP/Services/Repositories", $"{rep}.cs");
        if (!File.Exists(datei))
          File.WriteAllText(datei, s);
      }
    }

    string GetToolItemName(XAttribute a)
    {
      var name = a.Value == "aktuell" || a.Value == "berechnen" ? "refresh" :
        a.Value == "rueckgaengig" ? "undo" :
        a.Value == "wiederherstellen" ? "redo" :
        a.Value == "neu" ? "new" :
        a.Value == "kopieren" ? "copy" :
        a.Value == "einfuegen" ? "paste" :
        a.Value == "aendern" ? "edit" :
        a.Value == "loeschen" ? "delete" :
        a.Value == "export" ? "save" :
        a.Value == "drucken" ? "print" :
        a.Value == "imExport" ? "floppy" :
        null;
      return name;
    }

    /// <summary>Kontext zum Generieren eines Formulars.</summary>
    class GeneratorContext
    {
      public int idcount { get; set; } = 0;
      public XNamespace ns { get; set; }
      public XNamespace fxns { get; set; }
      public string fileshort { get; set; }
      public string filenew { get; set; }
      public StringBuilder events { get; set; } = new StringBuilder();
      public StringBuilder ms { get; set; } = new StringBuilder();
      public StringBuilder member { get; set; } = new StringBuilder();
      public StringBuilder init { get; set; } = new StringBuilder();
    }

    /// <summary>Generieren eines Formulars aus einer JavaFX-Datei.</summary>
    [Test]
    public void GenerateForm()
    {
      var unit = "so";
      var fileold = "SO100Sudoku";
      var filenew = "SO100Sudoku";
      var fileshort = filenew.Substring(0, 5);
      var file = Path.Combine(formpath, unit, fileold + ".fxml");
      var g = XDocument.Load(file);
      var root = g.Root; // GridPane
      var doc = new XDocument();
      doc.Add(new XComment(" Created with unit test GenerateForm "));
      var w = new XElement("interface", new XElement("requires", new XAttribute("lib", "gtk+"), new XAttribute("version", "3.20")));
      doc.Add(w);
      XNamespace ns0 = "http://www.w3.org/2000/xmlns/";
      var xmlns = root.Attributes().FirstOrDefault(a => a.Name == "xmlns");
      XNamespace ns = xmlns?.Value ?? "http://javafx.com/javafx/8.0.45";
      xmlns = root.Attributes().FirstOrDefault(a => a.Name == ns0 + "fx");
      XNamespace fxns = xmlns?.Value ?? "http://javafx.com/fxml/1";
      var gc = new GeneratorContext
      {
        ns = ns,
        fxns = fxns,
        fileshort = fileshort,
        filenew = filenew,
      };

      // Bilder für Toolbar-Buttons
      var tbbuttons = root.Descendants(ns + "Button")
        .Where(a => a.Parent.Name == ns + "items" && a.Parent.Parent.Name == ns + "ToolBar").ToList();
      foreach (XElement b in tbbuttons)
      {
        var a = b.Attribute(fxns + "id");
        var name = GetToolItemName(a);
        if (name != null)
        {
          w.Add(new XElement("object", new XAttribute("class", "GtkImage"), new XAttribute("id", $"{name}Image"),
            new XElement("property", new XAttribute("name", "visible"), new XText("True")),
            new XElement("property", new XAttribute("name", "can_focus"), new XText("False")),
            new XElement("property", new XAttribute("name", "stock"), new XText($"gtk-{name}"))
          ));
        }
        Console.WriteLine(a.Value);
      }
      gc.ms.Append($@"
    <data name=""{fileshort}.title"" xml:space=""preserve"">
        <value>{fileshort}.title</value>
    </data>");
      AddElement(gc, root, w, 0);
      var xmlfile = Path.Combine(pfad, $"csbp/CSBP/GtkGui/{unit.ToUpper()}", filenew + ".glade");
      //var settings = new XmlWriterSettings { OmitXmlDeclaration = false, Indent = true, Encoding = Encoding.UTF8 };
      //using (var xw = XmlWriter.Create(xmlfile, settings))
      //  doc.Save(xw); // UTF-8 mit BOM
      using (var writer = new XmlTextWriter(xmlfile, new UTF8Encoding(false)))
      {
        writer.Formatting = Formatting.Indented;
        //writer.Indentation = 2;
        doc.Save(writer);
      }
      var s = $@"// <copyright file=""{filenew}.cs"" company=""cwkuehl.de"">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.{unit.ToUpper()}
{{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für {filenew} Dialog.</summary>
  public partial class {filenew} : CsbpBin
  {{
    /// <summary>Dialog Model.</summary>
    private MaMandant Model;

#pragma warning disable 169, 649{gc.member}

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name=""p1"">1. Parameter für Dialog.</param>
    /// <param name=""p"">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static {filenew} Create(object p1 = null, CsbpBin p = null)
    {{
      return new {filenew}(GetBuilder(""{filenew}"", out var handle), handle, p1: p1, p: p);
    }}

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name=""builder"">Betroffener Builder.</param>
    /// <param name=""h"">Betroffenes Handle vom Builder.</param>
    /// <param name=""d"">Betroffener einbettender Dialog.</param>
    /// <param name=""dt"">Betroffener Dialogtyp.</param>
    /// <param name=""p1"">1. Parameter für Dialog.</param>
    /// <param name=""p"">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public {filenew}(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {{{gc.init}
      // SetBold(client0);
      Functions.MachNichts(Model);
      InitData(0);
    }}

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name=""step"">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {{
      if (step <= 0) {{
      }}
    }}{gc.events}
  }}
}}
";
      var datei = Path.Combine(pfad, $"csbp/CSBP/Forms/{unit.ToUpper()}", $"{filenew}.cs");
      if (!File.Exists(datei))
      {
        File.WriteAllText(datei, s);
        File.AppendAllText(datei, Constants.CRLF + gc.ms + Constants.CRLF);
      }
      // File.AppendAllText(xmlfile, Constants.CRLF);
      // if (Functions.MachNichts() == 0)
      //   File.AppendAllText(xmlfile, Constants.CRLF + ms + Constants.CRLF);
      // Process.Start(xmlfile);
    }

    void AddElement(GeneratorContext gc, XElement source, XElement target, int depth)
    {
      XElement e0 = null; // Element wird zum target hinzugefügt.
      var id = source.Attributes().FirstOrDefault(a => a.Name == gc.fxns + "id")?.Value ?? $"id{++gc.idcount}";
      var rowIndex = source.Attributes().FirstOrDefault(a => a.Name == "GridPane.rowIndex")?.Value;
      var rowSpan = source.Attributes().FirstOrDefault(a => a.Name == "GridPane.rowSpan")?.Value;
      var columnIndex = source.Attributes().FirstOrDefault(a => a.Name == "GridPane.columnIndex")?.Value;
      var columnSpan = source.Attributes().FirstOrDefault(a => a.Name == "GridPane.columnSpan")?.Value;
      //var xexpand = true;
      //var yexpand = false;
      var addchild = true;
      var children = true;
      if (source.Name == gc.ns + "Label")
      {
        //xexpand = false;
        var text = source.Attributes().FirstOrDefault(a => a.Name == "text")?.Value ?? $"{gc.fileshort}.{id}";
        if (text.EndsWith("Creation", StringComparison.CurrentCulture))
          text = "Forms.created0";
        else if (text.EndsWith("Change", StringComparison.CurrentCulture))
          text = "Forms.changed0";
        text = text.StartsWith("%", StringComparison.CurrentCulture) ? text.Substring(1) : text;
        var text0 = text.EndsWith("0", StringComparison.CurrentCulture) ? text.Substring(0, text.Length - 1) : text;
        gc.ms.Append($@"
    <data name=""{text0}"" xml:space=""preserve"">
        <value>_{text0}</value>
    </data>");
        gc.member.Append($@"

    /// <summary>Label {id}.</summary>
    [Builder.Object]
    private Label {id};");
        e0 = new XElement("object", new XAttribute("class", "GtkLabel"), new XAttribute("id", id),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("False")),
          new XElement("property", new XAttribute("name", "halign"), new XText("start")),
          new XElement("property", new XAttribute("name", "valign"), new XText("center")),
          new XElement("property", new XAttribute("name", "label"), new XText(text0)),
          new XElement("property", new XAttribute("name", "use_underline"), new XText("True"))
        );
        var x = source.NextNode as XElement; // Nächstes Element mit Label verknüpfen.
        var idx = x?.Attributes().FirstOrDefault(a => a.Name == gc.fxns + "id");
        var prop = idx?.Value ?? id;
        if (idx != null)
          e0.Add(new XElement("property", new XAttribute("name", "mnemonic_widget"), new XText(prop)));
      }
      else if (source.Name == gc.ns + "TextField" || source.Name == gc.ns + "PasswordField")
      {
        var prompttext = source.Attributes().FirstOrDefault(a => a.Name == gc.ns + "promptText")?.Value ?? $"{gc.fileshort}.{id}.tt";
        prompttext = prompttext.StartsWith("%", StringComparison.CurrentCulture) ? prompttext.Substring(1) : prompttext;
        if (prompttext.EndsWith("angelegt.tt", StringComparison.CurrentCulture))
          prompttext = "Forms.created.tt";
        else if (prompttext.EndsWith("geaendert.tt", StringComparison.CurrentCulture))
          prompttext = "Forms.changed.tt";
        else
          gc.ms.Append($@"
    <data name=""{gc.fileshort}.{id}.tt"" xml:space=""preserve"">
        <value>{gc.fileshort}.{id}.tt</value>
    </data>");
        gc.member.Append($@"

    /// <summary>Entry {id}.</summary>
    [Builder.Object]
    private Entry {id};");
        e0 = new XElement("object", new XAttribute("class", "GtkEntry"), new XAttribute("id", id),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
          new XElement("property", new XAttribute("name", "tooltip_text"), new XText(prompttext)),
          new XElement("property", new XAttribute("name", "valign"), new XText("start")),
          new XElement("property", new XAttribute("name", "hexpand"), new XText("True")),
          new XElement("property", new XAttribute("name", "activates_default"), new XText("True")),
          new XElement("property", new XAttribute("name", "placeholder_text"), new XText(prompttext))
        );
        if (source.Name == gc.ns + "PasswordField")
        {
          e0.Add(new XElement("property", new XAttribute("name", "visibility"), new XText("False")));
          e0.Add(new XElement("property", new XAttribute("name", "input_purpose"), new XText("password")));
        }
      }
      else if (source.Name == gc.ns + "TextArea")
      {
        var prompttext = source.Attributes().FirstOrDefault(a => a.Name == gc.ns + "promptText")?.Value ?? $"{gc.fileshort}.{id}.tt";
        prompttext = prompttext.StartsWith("%", StringComparison.CurrentCulture) ? prompttext.Substring(1) : prompttext;
        if (prompttext.EndsWith("angelegt.tt", StringComparison.CurrentCulture))
          prompttext = "Forms.created.tt";
        else if (prompttext.EndsWith("geaendert.tt", StringComparison.CurrentCulture))
          prompttext = "Forms.changed.tt";
        else
          gc.ms.Append($@"
    <data name=""{gc.fileshort}.{id}.tt"" xml:space=""preserve"">
        <value>{gc.fileshort}.{id}.tt</value>
    </data>");
        gc.member.Append($@"

    /// <summary>TextView {id}.</summary>
    [Builder.Object]
    private TextView {id};");
        e0 = new XElement("object", new XAttribute("class", "GtkScrolledWindow"), new XAttribute("id", $"{id}sw"),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
          new XElement("property", new XAttribute("name", "tooltip_text"), new XText($"{gc.fileshort}.{id}.tt")),
          new XElement("property", new XAttribute("name", "shadow_type"), new XText("in")),
          new XElement("child",
            new XElement("object", new XAttribute("class", "GtkTextView"), new XAttribute("id", id),
              new XElement("property", new XAttribute("name", "visible"), new XText("True")),
              new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
              new XElement("property", new XAttribute("name", "sensitive"), new XText("True")),
              new XElement("property", new XAttribute("name", "wrap_mode"), new XText("word")),
              new XElement("property", new XAttribute("name", "hscroll_policy"), new XText("natural"))
            )
          )
        );
      }
      else if (source.Name == gc.ns + "ComboBox")
      {
        var prompttext = source.Attributes().FirstOrDefault(a => a.Name == gc.ns + "accessibleText")?.Value ?? $"{gc.fileshort}.{id}.tt";
        prompttext = prompttext.StartsWith("%", StringComparison.CurrentCulture) ? prompttext.Substring(1) : prompttext;
        e0 = new XElement("object", new XAttribute("class", "GtkComboBoxText"), new XAttribute("id", id),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("False")),
          new XElement("property", new XAttribute("name", "tooltip_text"), new XText($"{gc.fileshort}.{id}.tt")),
          new XElement("property", new XAttribute("name", "has_entry"), new XText("True")),
          // new XElement("property", new XAttribute("name", "Items"), new XAttribute("translatable", "yes")),
          // new XElement("property", new XAttribute("name", "IsTextCombo"), new XText("True")),
          new XElement("signal", new XAttribute("name", "changed"), new XAttribute("handler", $"On{id.ToFirstUpper()}Changed")),
          new XElement("child", new XAttribute("internal-child", "entry"),
            new XElement("object", new XAttribute("class", "GtkEntry"),
              new XElement("property", new XAttribute("name", "visible"), new XText("True")),
              new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
              new XElement("property", new XAttribute("name", "text"))
            )
          )
        );
        gc.ms.Append($@"
    <data name=""{gc.fileshort}.{id}.tt"" xml:space=""preserve"">
        <value>{gc.fileshort}.{id}.tt</value>
    </data>");
        gc.member.Append($@"

    /// <summary>ComboBox {id}.</summary>
    [Builder.Object]
    private ComboBox {id};");
        gc.events.Append($@"

    /// <summary>Behandlung von {id.ToFirstUpper()}.</summary>
    /// <param name=""sender"">Betroffener Sender.</param>
    /// <param name=""e"">Betroffenes Ereignis.</param>
    protected void On{id.ToFirstUpper()}Changed(object sender, EventArgs e)
    {{
    }}");
      }
      else if (source.Name == gc.ns + "CheckBox")
      {
        var prompttext = source.Attributes().FirstOrDefault(a => a.Name == gc.ns + "text")?.Value ?? $"{gc.fileshort}.{id}";
        prompttext = prompttext.StartsWith("%", StringComparison.CurrentCulture) ? prompttext.Substring(1) : prompttext;
        e0 = new XElement("object", new XAttribute("class", "GtkCheckButton"), new XAttribute("id", id),
            new XElement("property", new XAttribute("name", "label"), new XText(prompttext)),
            new XElement("property", new XAttribute("name", "visible"), new XText("True")),
            new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
            new XElement("property", new XAttribute("name", "tooltip_text"), new XText($"{gc.fileshort}.{id}.tt")),
            new XElement("property", new XAttribute("name", "valign"), new XText("start")),
            new XElement("property", new XAttribute("name", "hexpand"), new XText("True")),
            new XElement("property", new XAttribute("name", "draw_indicator"), new XText("True")),
            new XElement("property", new XAttribute("name", "use_underline"), new XText("True"))
            );
        gc.ms.Append($@"
    <data name=""{gc.fileshort}.{id}"" xml:space=""preserve"">
        <value>_{gc.fileshort}.{id}</value>
    </data>");
        gc.ms.Append($@"
    <data name=""{gc.fileshort}.{id}.tt"" xml:space=""preserve"">
        <value>{gc.fileshort}.{id}.tt</value>
    </data>");
        gc.member.Append($@"

    /// <summary>CheckButton {id}.</summary>
    [Builder.Object]
    private CheckButton {id};");
      }
      else if (source.Name == gc.ns + "Button")
      {
        var df = source.Attribute("defaultButton")?.Value == "true";
        var text = source.Attributes().FirstOrDefault(a => a.Name == "text")?.Value ?? $"{gc.fileshort}.{id}";
        text = text.StartsWith("%", StringComparison.CurrentCulture) ? text.Substring(1) : text;
        if (text.EndsWith("Ok", StringComparison.CurrentCulture))
          text = "Forms.ok";
        else if (text.EndsWith("Cancel", StringComparison.CurrentCulture))
          text = "Forms.cancel";
        else
          gc.ms.Append($@"
    <data name=""{text}"" xml:space=""preserve"">
        <value>{text}</value>
    </data>").Append($@"
    <data name=""{text}.tt"" xml:space=""preserve"">
        <value>{text}.tt</value>
    </data>");
        gc.member.Append($@"

    /// <summary>Button {id}.</summary>
    [Builder.Object]
    private Button {id};");
        e0 = new XElement("object", new XAttribute("class", "GtkButton"), new XAttribute("id", id),
          new XElement("property", new XAttribute("name", "label"), new XText(text)),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_default"), new XText(df ? "True" : "False")),
          new XElement("property", new XAttribute("name", "tooltip_text"), new XText($"{text}.tt")),
          new XElement("property", new XAttribute("name", "use_underline"), new XText("True")),
          new XElement("signal", new XAttribute("name", "clicked"), new XAttribute("handler", $"On{id.ToFirstUpper()}Clicked"), new XAttribute("swapped", "no"))
        );
        gc.events.Append($@"

    /// <summary>Behandlung von {id.ToFirstUpper()}.</summary>
    /// <param name=""sender"">Betroffener Sender.</param>
    /// <param name=""e"">Betroffenes Ereignis.</param>
    protected void On{id.ToFirstUpper()}Clicked(object sender, EventArgs e)
    {{
    }}");
      }
      else if (source.Name == gc.ns + "HBox" || source.Name == gc.ns + "VBox")
      {
        e0 = new XElement("object", new XAttribute("class", "GtkBox"), new XAttribute("id", id),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("False")),
          new XElement("property", new XAttribute("name", "hexpand"), new XText("False")),
          new XElement("property", new XAttribute("name", "spacing"), new XText("5"))
        );
        if (source.Name == gc.ns + "VBox")
          e0.Add(new XElement("property", new XAttribute("name", "orientation"), new XText("vertical")));
        //     children = false;
        var nr = 1;
        //     var buttons = source.Descendants(gc.ns + "Button");
        //     foreach (var b in buttons)
        //     {
        //       var idb = b.Attribute(gc.fxns + "id")?.Value;
        //       var df = b.Attribute("defaultButton")?.Value == "true";
        //       var text = b.Attributes().FirstOrDefault(a => a.Name == "text")?.Value ?? $"{gc.fileshort}.button{nr}";
        //       text = text.StartsWith("%", StringComparison.CurrentCulture) ? text.Substring(1) : text;
        //       if (text.EndsWith("Ok", StringComparison.CurrentCulture))
        //         text = "Forms.ok";
        //       else if (text.EndsWith("Cancel", StringComparison.CurrentCulture))
        //         text = "Forms.cancel";
        //       else
        //         gc.ms.Append($@"
        // <data name=""{text}"" xml:space=""preserve"">
        //     <value>{text}</value>
        // </data>").Append($@"
        // <data name=""{text}.tt"" xml:space=""preserve"">
        //     <value>{text}.tt</value>
        // </data>");
        //       var eb = new XElement("child",
        //         new XElement("object", new XAttribute("class", "GtkButton"), new XAttribute("id", idb),
        //           new XElement("property", new XAttribute("name", "label"), new XText(text)),
        //           new XElement("property", new XAttribute("name", "visible"), new XText("True")),
        //           new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
        //           new XElement("property", new XAttribute("name", "can_default"), new XText(df ? "True" : "False")),
        //           new XElement("property", new XAttribute("name", "tooltip_text"), new XText($"{text}.tt")),
        //           new XElement("property", new XAttribute("name", "use_underline"), new XText("True")),
        //           new XElement("signal", new XAttribute("name", "clicked"), new XAttribute("handler", $"On{idb.ToFirstUpper()}Clicked"), new XAttribute("swapped", "no"))),
        //         new XElement("packing",
        //           new XElement("property", new XAttribute("name", "expand"), new XText("False")),
        //           new XElement("property", new XAttribute("name", "fill"), new XText("True")),
        //           new XElement("property", new XAttribute("name", "position"), new XText((nr - 1).ToString())))
        //       );
        //       e0.Add(eb);
        //       gc.events.Append($@"

        // /// <summary>Behandlung von {idb.ToFirstUpper()}.</summary>
        // /// <param name=""sender"">Betroffener Sender.</param>
        // /// <param name=""e"">Betroffenes Ereignis.</param>
        // protected void On{idb.ToFirstUpper()}Clicked(object sender, EventArgs e)
        // {{
        // }}");
        //       gc.member.Append($@"

        // /// <summary>Button {idb}.</summary>
        // [Builder.Object]
        // private Button {idb};");
        //       nr++;
        //     }
        var togglegroup = source.Descendants(gc.ns + "ToggleGroup")
                                .FirstOrDefault(a => a.Parent.Name == gc.fxns + "define");
        var tg = togglegroup == null ? null : (togglegroup.Attributes().FirstOrDefault(a => a.Name == gc.fxns + "id")?.Value ?? $"tg{id}");
        if (tg != null)
        {
          children = false;
          var rbs = source.Descendants(gc.ns + "RadioButton");
          foreach (var rb in rbs)
          {
            var text = rb.Attributes().FirstOrDefault(a => a.Name == "text")?.Value ?? $"{gc.fileshort}.{tg}{nr}";
            text = text.StartsWith("%", StringComparison.CurrentCulture) ? text.Substring(1) : text;
            gc.ms.Append($@"
    <data name=""{text}"" xml:space=""preserve"">
        <value>{text}</value>
    </data>").Append($@"
    <data name=""{text}.tt"" xml:space=""preserve"">
        <value>{text}.tt</value>
    </data>");
            var erb = new XElement("child",
              new XElement("object", new XAttribute("class", "GtkRadioButton"), new XAttribute("id", $"{tg}{nr}"),
                new XElement("property", new XAttribute("name", "label"), new XText(text)),
                new XElement("property", new XAttribute("name", "use_underline"), new XText("True")),
                new XElement("property", new XAttribute("name", "visible"), new XText("True")),
                new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
                new XElement("property", new XAttribute("name", "tooltip_text"), new XText($"{text}.tt")),
                new XElement("property", new XAttribute("name", "receives_default"), new XText("False")),
                new XElement("property", new XAttribute("name", "active"), new XText(nr == 1 ? "True" : "False")),
                new XElement("property", new XAttribute("name", "draw_indicator"), new XText("True")),
                new XElement("property", new XAttribute("name", "group"), new XText($"{tg}1"))),
              new XElement("packing",
                new XElement("property", new XAttribute("name", "expand"), new XText("False")),
                new XElement("property", new XAttribute("name", "fill"), new XText("True")),
                new XElement("property", new XAttribute("name", "position"), new XText((nr - 1).ToString()))
              )
            );
            // if (nr == 0)
            gc.member.Append($@"

    /// <summary>RadioButton {tg}{nr}.</summary>
    [Builder.Object]
    private RadioButton {tg}{nr};");
            e0.Add(erb);
            nr++;
          }
        }
      }
      else if (source.Name == gc.ns + "GridPane")
      {
        if (depth <= 0)
          addchild = false;
        e0 = new XElement("object", new XAttribute("class", "GtkGrid"), new XAttribute("id", depth <= 0 ? gc.filenew : id),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("False")),
          new XElement("property", new XAttribute("name", "margin_left"), new XText("5")),
          new XElement("property", new XAttribute("name", "margin_right"), new XText("5")),
          new XElement("property", new XAttribute("name", "margin_top"), new XText("5")),
          new XElement("property", new XAttribute("name", "margin_bottom"), new XText("5")),
          new XElement("property", new XAttribute("name", "hexpand"), new XText("True")),
          new XElement("property", new XAttribute("name", "vexpand"), new XText("True")),
          new XElement("property", new XAttribute("name", "row_spacing"), new XText("5")),
          new XElement("property", new XAttribute("name", "column_spacing"), new XText("5"))
        );
        //yexpand = true;
      }
      else if (source.Name == gc.ns + "TableView" || source.Name == gc.ns + "ListView")
      {
        gc.ms.Append($@"
    <data name=""{gc.fileshort}.{id}.tt"" xml:space=""preserve"">
        <value>{gc.fileshort}.{id}.tt</value>
    </data>").Append($@"
    <data name=""{gc.fileshort}.{id}.columns"" xml:space=""preserve"">
        <value>{gc.fileshort}.{id}.columns;Geändert am;Geändert von;Angelegt am;Angelegt von</value>
    </data>");
        e0 = new XElement("object", new XAttribute("class", "GtkScrolledWindow"), new XAttribute("id", $"{id}sw"),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
          new XElement("property", new XAttribute("name", "vexpand"), new XText("True")),
          new XElement("property", new XAttribute("name", "hexpand"), new XText("True")),
          new XElement("property", new XAttribute("name", "shadow_type"), new XText("in")),
          new XElement("child",
            new XElement("object", new XAttribute("class", "GtkTreeView"), new XAttribute("id", id),
            new XElement("property", new XAttribute("name", "visible"), new XText("True")),
            new XElement("property", new XAttribute("name", "can_focus"), new XText("True")),
            new XElement("property", new XAttribute("name", "tooltip_text"), new XText($"{gc.fileshort}.{id}.tt")),
            new XElement("signal", new XAttribute("name", "row-activated"), new XAttribute("handler", $"On{id.ToFirstUpper()}RowActivated"), new XAttribute("swapped", "no"))
          ))
        );
        gc.member.Append($@"

    /// <summary>TreeView {id}.</summary>
    [Builder.Object]
    private TreeView {id};");
        gc.events.Append($@"

    /// <summary>Behandlung von {id.ToFirstUpper()}.</summary>
    /// <param name=""sender"">Betroffener Sender.</param>
    /// <param name=""e"">Betroffenes Ereignis.</param>
    protected void On{id.ToFirstUpper()}RowActivated(object sender, RowActivatedArgs e)
    {{
    }}");
        //yexpand = true;
      }
      else if (source.Name == gc.ns + "ToolBar")
      {
        var tbbuttons = source.Descendants(gc.ns + "Button")
            .Where(a => a.Parent.Name == gc.ns + "items" && a.Parent.Parent.Name == gc.ns + "ToolBar")
            .ToList();
        e0 = new XElement("object", new XAttribute("class", "GtkActionBar"), new XAttribute("id", id),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("False")),
          new XElement("property", new XAttribute("name", "hexpand"), new XText("True"))
        );
        var nr = 0;
        foreach (XElement b in tbbuttons)
        {
          var a = b.Attribute(gc.fxns + "id");
          var name = GetToolItemName(a);
          if (name != null)
          {
            e0.Add(new XElement("child",
              new XElement("object", new XAttribute("class", "GtkButton"), new XAttribute("id", $"{name}Action"),
                // new XElement("property", new XAttribute("name", "label"), new XText($"")),
                new XElement("property", new XAttribute("name", "visible"), new XText("True")),
                new XElement("property", new XAttribute("name", "can_focus"), new XText("False")),
                new XElement("property", new XAttribute("name", "receives_default"), new XText("True")),
                new XElement("property", new XAttribute("name", "tooltip_text"), new XText($"Action.{name}")),
                new XElement("property", new XAttribute("name", "image"), new XText($"{name}Image")),
                new XElement("property", new XAttribute("name", "always_show_image"), new XText("True")),
                new XElement("signal", new XAttribute("name", "clicked"), new XAttribute("handler", $"On{name.ToFirstUpper()}Clicked"), new XAttribute("swapped", "no"))
              ),
              new XElement("packing",
                new XElement("property", new XAttribute("name", "position"), new XText(nr.ToString()))
              )
            ));
            gc.member.Append($@"

    /// <summary>Button {name.ToFirstUpper()}Action.</summary>
    [Builder.Object]
    private Button {name}Action;");
            gc.events.Append($@"

    /// <summary>Behandlung von {name.ToFirstUpper()}.</summary>
    /// <param name=""sender"">Betroffener Sender.</param>
    /// <param name=""e"">Betroffenes Ereignis.</param>
    protected void On{name.ToFirstUpper()}Clicked(object sender, EventArgs e)
    {{
    }}");
          }
          nr++;
        }
      }
      else if (source.Name == gc.ns + "SplitPane")
      {
        source = source.FirstNode as XElement;
        e0 = new XElement("object", new XAttribute("class", "GtkPaned"), new XAttribute("id", id),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("False")),
          new XElement("property", new XAttribute("name", "orientation"), new XText("vertical")),
          new XElement("property", new XAttribute("name", "position"), new XText("50"))
        );
        gc.member.Append($@"

    /// <summary>Paned {id}.</summary>
    [Builder.Object]
    private Paned {id};");
      }
      else if (source.Name == gc.ns + "AnchorPane")
      {
        var node = (source.FirstNode as XElement).FirstNode as XElement;
        AddElement(gc, node, target, depth + 1);
      }
      else if (source.Name == gc.ns + "Datum")
      {
        gc.ms.Append($@"
    <data name=""{gc.fileshort}.{id}.tt"" xml:space=""preserve"">
        <value>{gc.fileshort}.{id}.tt</value>
    </data>");
        var prompttext = $"{gc.fileshort}.{id}.tt";
        e0 = new XElement("object", new XAttribute("class", "GtkGrid"), new XAttribute("id", id),
          new XElement("property", new XAttribute("name", "visible"), new XText("True")),
          new XElement("property", new XAttribute("name", "can_focus"), new XText("False")),
          new XElement("property", new XAttribute("name", "visible"), new XText("True"))
        );
        gc.init.Append($@"
      {id} = new Date(Builder.GetObject(""{id}"").Handle)
      {{
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      }};
      {id}.DateChanged += On{id.ToFirstUpper()}DateChanged;
      {id}.Show();");
        gc.events.Append($@"

    /// <summary>Behandlung von {id}.</summary>
    /// <param name=""sender"">Betroffener Sender.</param>
    /// <param name=""e"">Betroffenes Ereignis.</param>
    protected void On{id.ToFirstUpper()}DateChanged(object sender, DateChangedEventArgs e)
    {{
    }}");
        gc.member.Append($@"

    /// <summary>Date {id.ToFirstUpper()}.</summary>
    //[Builder.Object]
    private Date {id};");
      }
      if (e0 != null)
      {
        if (addchild)
        {
          var child = new XElement("child");
          child.Add(e0);
          target.Add(child);
        }
        else
          target.Add(e0);
        if (rowIndex != null || rowSpan != null || columnIndex != null || columnSpan != null)
        {
          var r = Functions.ToInt32(rowIndex);
          var rs = Math.Max(Functions.ToInt32(rowSpan), 1);
          var c = Functions.ToInt32(columnIndex);
          var cs = Math.Max(Functions.ToInt32(columnSpan), 1);
          var p = new XElement("packing",
            new XElement("property", new XAttribute("name", "left_attach"), new XText(c.ToString())),
            new XElement("property", new XAttribute("name", "top_attach"), new XText(r.ToString()))
          //new XElement("property", new XAttribute("name", "BottomAttach"), new XText((r + rs).ToString())),
          //new XElement("property", new XAttribute("name", "RightAttach"), new XText((c + cs).ToString())),
          //new XElement("property", new XAttribute("name", "AutoSize"), new XText("False")),
          //new XElement("property", new XAttribute("name", "XExpand"), new XText(xexpand.ToString().ToFirstUpper())),
          //new XElement("property", new XAttribute("name", "XFill"), new XText(true.ToString().ToFirstUpper())),
          //new XElement("property", new XAttribute("name", "YExpand"), new XText(yexpand.ToString().ToFirstUpper())),
          //new XElement("property", new XAttribute("name", "YFill"), new XText(true.ToString().ToFirstUpper()))
          );
          if (!string.IsNullOrEmpty(columnSpan))
            p.Add(new XElement("property", new XAttribute("name", "width"), new XText(columnSpan)));
          if (!string.IsNullOrEmpty(rowSpan))
            p.Add(new XElement("property", new XAttribute("name", "height"), new XText(rowSpan)));
          e0.AddAfterSelf(p);
        }
        if (children)
        {
          foreach (XElement node in source.Nodes())
          {
            AddElement(gc, node, e0, depth + 1);
          }
        }
      }
    }

    /// <summary>Generieren der .Designer.cs-Datei aus .resx-Datei.</summary>
    [Test]
    public void GenerateResxDesigner()
    {
      var fn = "CSBP/Resources/Messages.resx";
      var slnpfad = "/home/wolfgang/cs/csbp";
      var ns = Path.GetDirectoryName(fn).Replace('/', '.');
      var filename = Path.GetFileNameWithoutExtension(fn);
      var values = new StringBuilder();
      values.Append($@"

    // private static System.Globalization.CultureInfo rc;

    // internal static System.Globalization.CultureInfo Culture
    // {{
    //   get {{ return rc; }}
    //   set {{ rc = value; }}
    // }}

    public static string Get(string key)
    {{
      return rm.GetString(key);
    }}");
      var g = XDocument.Load(Path.Combine(slnpfad, fn));
      var datas = g.Descendants("data");
      foreach (var d in datas)
      {
        var name = d.Attribute("name").Value;
        var namecs = name.Replace('.', '_');
        //var value = (d.Descendants("value").FirstOrDefault()?.FirstNode as XText)?.Value;
        values.Append(Constants.CRLF).Append(Constants.CRLF);
        values.Append($@"    public static string {namecs}
    {{
      get {{ return rm.GetString(""{name}""); }}
    }}");
      }
      var s = $@"// <copyright file=""{filename}.cs"" company=""cwkuehl.de"">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace {ns}
{{
  using System;

  /// <summary>
  /// Resource class for {filename}.
  /// </summary>
  public partial class {filename}
  {{
    private static System.Resources.ResourceManager rm =
      new System.Resources.ResourceManager(""{ns}.{filename}"", typeof({filename}).Assembly);{values}
  }}
}}
";
      var datei = Path.Combine(slnpfad, $"CSBP/Resources", $"{filename}.cs");
      File.WriteAllText(datei, s);
    }

    /// <summary>Tests für TLS, HTTPS-Server.</summary>
    [Test]
    public void Tls()
    {
      // var cert = new X509Certificate2("/opt/Haushalt/CSBP/cert/cert_key.pfx", "", X509KeyStorageFlags.MachineKeySet);
      var cert = new X509Certificate2("/opt/Haushalt/CSBP/cert/cert_key.pfx", "");
      // var date = DateTime.Today;
      // var shortcut = "USD";
      // var accesskey = "4b1846822a23c8e9d4ad08c0e30f28f4";
      // var url = $"http://data.fixer.io/api/{Functions.ToString(date)}?symbols={shortcut}&access_key={accesskey}";
      var url = "http://data.fixer.io/api/2020-10-01?symbols=USD&access_key=4b1846822a23c8e9d4ad08c0e30f28f4";
      // var url = "https://www.onvista.de/fonds/snapshotHistoryCSV?idNotation=295567847&datetimeTzStartRange=24.09.2020&timeSpan=7D&codeResolution=1D";
      System.Net.ServicePointManager.SecurityProtocol = /*System.Net.SecurityProtocolType.Tls13 |*/ System.Net.SecurityProtocolType.Tls12;
      var httpsclient = new System.Net.Http.HttpClient();
      httpsclient.Timeout = TimeSpan.FromMilliseconds(5000);
      // httpsclient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
      if (Functions.MachNichts() == 0)
      {
        // The request was canceled due to the configured HttpClient.Timeout of 5 seconds elapsing.
        url = "https://www.onvista.de/onvista/boxes/historicalquote/export.csv?notationId=9385716&dateStart=20.08.2021&interval=Y1";
        // url = "https://query1.finance.yahoo.com/v7/finance/chart/GC=F?period1=1628294400&period2=1628899200&interval=1d&indicators=quote&includeTimestamps=true";
        // httpsclient.Timeout = TimeSpan.FromMilliseconds(10000);
        httpsclient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:97.0) Gecko/20100101 Firefox/97.0");
        var wr = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
        var s = httpsclient.Send(wr);
      }
      else
      {
        var task = httpsclient.GetStringAsync(url);
        task.Wait();
      }
    }

    /// <summary>Tests für Serialisierung.</summary>
    [Test]
    public void Serialize()
    {
      var l = new List<BackupEntry>();
      l.Add(new BackupEntry { Uid = "abc" });
      var bytes = Functions.Serialize(l);
      var v = Convert.ToBase64String(bytes);
      var bytes2 = Convert.FromBase64String(v);
      var l2 = Functions.Deserialize<List<BackupEntry>>(bytes2);
    }
  }
}