using NXStartCenter.Services;

namespace NXStartCenter;

public sealed class MainForm : Form
{
    private readonly AppModel _model;
    private readonly NxService _nx;
    private readonly ProjectService _project;

    private readonly ComboBox _customer = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _version = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _machine = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _nativeVersion = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _postbuilderVersion = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _team = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _editor = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _theme = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly CheckBox _loadPp = new() { Text = "Postprocessor laden" };
    private readonly CheckBox _loadMachines = new() { Text = "Installed Machines laden" };
    private readonly CheckBox _loadTool = new() { Text = "Tool laden" };
    private readonly CheckBox _loadDevice = new() { Text = "Device laden" };
    private readonly CheckBox _loadFeed = new() { Text = "Feed/Speed laden" };
    private readonly Label _message = new() { AutoSize = false, Height = 42, Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleLeft };

    private readonly TextBox _nxPath = new();
    private readonly TextBox _customerEnvPath = new();
    private readonly TextBox _licencePath = new();
    private readonly TextBox _lmtoolsPath = new();
    private readonly TextBox _forkPath = new();
    private readonly TextBox _rolesPath = new();

    private readonly TextBox _newCustomer = new();
    private readonly TextBox _newVersion = new() { Text = "NX2506" };
    private readonly TextBox _newMachine = new();
    private readonly TextBox _newOrder = new() { Text = "0000" };
    private readonly ComboBox _newMachineType = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _newController = new() { DropDownStyle = ComboBoxStyle.DropDownList };

    public MainForm(AppModel model)
    {
        _model = model;
        _nx = new NxService(model);
        _project = new ProjectService(model);

        Text = "NX Startcenter";
        Width = 980;
        Height = 700;
        MinimumSize = new Size(900, 620);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10f);

        var menu = BuildMenu();
        Controls.Add(menu);
        MainMenuStrip = menu;

        var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(16, 8) };
        tabs.TabPages.Add(BuildStartTab());
        tabs.TabPages.Add(BuildCreateTab());
        tabs.TabPages.Add(BuildNativeTab());
        tabs.TabPages.Add(BuildSettingsTab());
        Controls.Add(tabs);

        _message.Text = "Bereit.";
        _message.Padding = new Padding(12, 0, 0, 0);
        Controls.Add(_message);

        LoadUiFromModel();
        ApplyTheme();
    }

    private MenuStrip BuildMenu()
    {
        var menu = new MenuStrip();
        var file = new ToolStripMenuItem("Datei");
        file.DropDownItems.Add("Beenden", null, (_, _) => Close());
        var tools = new ToolStripMenuItem("Tools");
        tools.DropDownItems.Add("Lizenz öffnen", null, (_, _) => Run(_nx.OpenLicenseFile));
        tools.DropDownItems.Add("LMTools starten", null, (_, _) => Run(_nx.StartLmTools));
        var info = new ToolStripMenuItem("Info");
        info.DropDownItems.Add("Über", null, (_, _) => MessageBox.Show($"NX Startcenter\nVersion: {_model.AppInfo.Version}\nDatum: {_model.AppInfo.AppDate}\nAutor: {_model.AppInfo.Author}\nSupport: {_model.AppInfo.SupportMail}", "Info"));
        menu.Items.AddRange([file, tools, info]);
        return menu;
    }

    private TabPage BuildStartTab()
    {
        var page = new TabPage("NX Kundenstart");
        var layout = TwoColumnPanel();
        AddCombo(layout, "Kunde", _customer, 0);
        AddCombo(layout, "NX Version", _version, 1);
        AddCombo(layout, "Maschine", _machine, 2);
        layout.Controls.Add(OptionsBox(), 0, 3);
        layout.SetColumnSpan(layout.GetControlFromPosition(0, 3)!, 2);
        var buttons = ButtonRow([
            ("NX starten", () => Run(_nx.StartCustomerNx)),
            ("Explorer", () => Run(_nx.OpenMachineFolder)),
            ("Fork", () => Run(_nx.OpenFork)),
            ("VS Code", () => Run(_nx.OpenVsCode)),
            ("Aktualisieren", RefreshEnvironment)
        ]);
        layout.Controls.Add(buttons, 0, 4);
        layout.SetColumnSpan(buttons, 2);
        page.Controls.Add(layout);
        return page;
    }

    private GroupBox OptionsBox()
    {
        var group = new GroupBox { Text = "Ladeoptionen", Dock = DockStyle.Top, Height = 110 };
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), WrapContents = true };
        flow.Controls.AddRange([_loadPp, _loadMachines, _loadTool, _loadDevice, _loadFeed]);
        group.Controls.Add(flow);
        return group;
    }

    private TabPage BuildCreateTab()
    {
        var page = new TabPage("Neues Projekt");
        var layout = TwoColumnPanel();
        AddText(layout, "Kunde", _newCustomer, 0);
        AddText(layout, "NX Version", _newVersion, 1);
        AddText(layout, "Maschine", _newMachine, 2);
        AddText(layout, "Auftrag", _newOrder, 3);
        AddCombo(layout, "Maschinentyp", _newMachineType, 4);
        AddCombo(layout, "Steuerung", _newController, 5);
        var buttons = ButtonRow([("Projekt anlegen", CreateProject)]);
        layout.Controls.Add(buttons, 0, 6);
        layout.SetColumnSpan(buttons, 2);
        page.Controls.Add(layout);
        return page;
    }

    private TabPage BuildNativeTab()
    {
        var page = new TabPage("Native / Postbuilder");
        var layout = TwoColumnPanel();
        AddCombo(layout, "Native NX Version", _nativeVersion, 0);
        AddCombo(layout, "Postbuilder Version", _postbuilderVersion, 1);
        var buttons = ButtonRow([
            ("Native NX starten", () => Run(_nx.StartNativeNx)),
            ("Postbuilder starten", () => Run(_nx.StartPostbuilder))
        ]);
        layout.Controls.Add(buttons, 0, 2);
        layout.SetColumnSpan(buttons, 2);
        page.Controls.Add(layout);
        return page;
    }

    private TabPage BuildSettingsTab()
    {
        var page = new TabPage("Einstellungen");
        var layout = TwoColumnPanel();
        AddText(layout, "NX Installationspfad", _nxPath, 0, true);
        AddText(layout, "Kundenumgebungen", _customerEnvPath, 1, true);
        AddText(layout, "Lizenzdatei", _licencePath, 2, true);
        AddText(layout, "LMTools", _lmtoolsPath, 3, true);
        AddText(layout, "Fork.exe", _forkPath, 4, true);
        AddText(layout, "Rollen (.mtx; getrennt)", _rolesPath, 5, true);
        AddCombo(layout, "Team", _team, 6);
        AddCombo(layout, "Editor", _editor, 7);
        AddCombo(layout, "Theme", _theme, 8);
        var buttons = ButtonRow([("Speichern", SaveSettings), ("Neu laden", RefreshEnvironment)]);
        layout.Controls.Add(buttons, 0, 9);
        layout.SetColumnSpan(buttons, 2);
        page.Controls.Add(layout);
        return page;
    }

    private static TableLayoutPanel TwoColumnPanel() => new()
    {
        Dock = DockStyle.Fill,
        Padding = new Padding(24),
        ColumnCount = 2,
        RowCount = 12,
        AutoScroll = true,
        ColumnStyles = { new ColumnStyle(SizeType.Absolute, 220), new ColumnStyle(SizeType.Percent, 100) }
    };

    private static void AddCombo(TableLayoutPanel layout, string label, ComboBox box, int row) => AddInput(layout, label, box, row);
    private static void AddText(TableLayoutPanel layout, string label, TextBox box, int row, bool wide = false) => AddInput(layout, label, box, row);

    private static void AddInput(TableLayoutPanel layout, string label, Control input, int row)
    {
        input.Dock = DockStyle.Top;
        input.Margin = new Padding(4, 8, 4, 8);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(4, 12, 4, 4) }, 0, row);
        layout.Controls.Add(input, 1, row);
    }

    private static FlowLayoutPanel ButtonRow((string Text, Action Action)[] specs)
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 54, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 12, 0, 0) };
        foreach (var spec in specs)
        {
            var b = new Button { Text = spec.Text, AutoSize = true, Height = 32, Margin = new Padding(4) };
            b.Click += (_, _) => spec.Action();
            panel.Controls.Add(b);
        }
        return panel;
    }

    private void LoadUiFromModel()
    {
        SetItems(_customer, _model.Customers, _model.Customer);
        SetItems(_version, _model.Versions, _model.VersionName);
        SetItems(_machine, _model.Machines, _model.Machine);
        SetItems(_nativeVersion, _model.NativeVersions, _model.NativeVersion);
        SetItems(_postbuilderVersion, _model.PostbuilderVersions, _model.PostbuilderVersion);
        SetItems(_newMachineType, _model.MachineTypes.Keys, _model.MachineTypes.Keys.First());
        SetItems(_newController, _model.MachineControllers, _model.MachineControllers.First());
        SetItems(_team, ["CAM", "PP"], _model.Settings.Team);
        SetItems(_editor, ["Notepad", "Notepad++", "VSCode"], _model.Settings.Editor);
        SetItems(_theme, ["darkly", "solar", "cyborg", "flatly", "cosmo", "minty", "pulse", "lumen"], _model.Settings.PreferredTheme);

        _loadPp.Checked = _model.Last.LastLoadPp != 0;
        _loadMachines.Checked = _model.Last.LastLoadInstalledMachines != 0;
        _loadTool.Checked = _model.Last.LastLoadTool != 0;
        _loadDevice.Checked = _model.Last.LastLoadDevice != 0;
        _loadFeed.Checked = _model.Last.LastLoadFeed != 0;

        _nxPath.Text = _model.Settings.NxInstallationPath;
        _customerEnvPath.Text = _model.Settings.CustomerEnvironmentPath;
        _licencePath.Text = _model.Settings.LicencePath;
        _lmtoolsPath.Text = _model.Settings.LicenceServerPath;
        _forkPath.Text = _model.Settings.ForkPath;
        _rolesPath.Text = _model.Settings.RolesPath;

        _customer.SelectedIndexChanged += (_, _) => { _model.Customer = _customer.Text; _model.RefreshVersions(); LoadCombo(_version, _model.Versions, _model.VersionName); LoadCombo(_machine, _model.Machines, _model.Machine); };
        _version.SelectedIndexChanged += (_, _) => { _model.VersionName = _version.Text; _model.RefreshMachines(); LoadCombo(_machine, _model.Machines, _model.Machine); };
        _machine.SelectedIndexChanged += (_, _) => _model.Machine = _machine.Text;
        _nativeVersion.SelectedIndexChanged += (_, _) => _model.NativeVersion = _nativeVersion.Text;
        _postbuilderVersion.SelectedIndexChanged += (_, _) => _model.PostbuilderVersion = _postbuilderVersion.Text;
        _theme.SelectedIndexChanged += (_, _) => ApplyTheme();
    }

    private static void SetItems(ComboBox box, IEnumerable<string> items, string? selected)
    {
        box.Items.Clear();
        box.Items.AddRange(items.Cast<object>().ToArray());
        if (!string.IsNullOrWhiteSpace(selected) && box.Items.Contains(selected)) box.SelectedItem = selected;
        else if (box.Items.Count > 0) box.SelectedIndex = 0;
    }

    private static void LoadCombo(ComboBox box, IEnumerable<string> items, string? selected) => SetItems(box, items, selected);

    private void SaveSettings()
    {
        _model.Settings.NxInstallationPath = _nxPath.Text.Trim();
        _model.Settings.CustomerEnvironmentPath = _customerEnvPath.Text.Trim();
        _model.Settings.LicencePath = _licencePath.Text.Trim();
        _model.Settings.LicenceServerPath = _lmtoolsPath.Text.Trim();
        _model.Settings.ForkPath = _forkPath.Text.Trim();
        _model.Settings.RolesPath = _rolesPath.Text.Trim();
        _model.Settings.Team = _team.Text;
        _model.Settings.Editor = _editor.Text;
        _model.Settings.PreferredTheme = _theme.Text;
        _model.Save();
        RefreshEnvironment();
        SetMessage("Einstellungen gespeichert.");
    }

    private void RefreshEnvironment()
    {
        _model.RefreshAll();
        LoadCombo(_customer, _model.Customers, _model.Customer);
        LoadCombo(_version, _model.Versions, _model.VersionName);
        LoadCombo(_machine, _model.Machines, _model.Machine);
        LoadCombo(_nativeVersion, _model.NativeVersions, _model.NativeVersion);
        LoadCombo(_postbuilderVersion, _model.PostbuilderVersions, _model.PostbuilderVersion);
        SetMessage("Umgebung aktualisiert.");
    }

    private void CreateProject()
    {
        try
        {
            var msg = _project.CreateCustomerProject(_newCustomer.Text.Trim(), _newVersion.Text.Trim(), _newMachine.Text.Trim(), _newOrder.Text.Trim(), _newMachineType.Text, _newController.Text);
            RefreshEnvironment();
            SetMessage(msg);
        }
        catch (Exception ex)
        {
            SetMessage("Fehler beim Anlegen des Projekts: " + ex.Message);
        }
    }

    private void Run(Func<string> action)
    {
        try { SetMessage(action()); }
        catch (Exception ex) { SetMessage(ex.Message); }
    }

    private void SetMessage(string msg) => _message.Text = msg;

    private void ApplyTheme()
    {
        var dark = _theme.Text is "darkly" or "solar" or "cyborg";
        var back = dark ? Color.FromArgb(32, 34, 37) : SystemColors.Control;
        var fore = dark ? Color.WhiteSmoke : SystemColors.ControlText;
        ApplyColors(this, back, fore);
        _message.BackColor = dark ? Color.FromArgb(45, 48, 54) : SystemColors.ControlLight;
    }

    private static void ApplyColors(Control c, Color back, Color fore)
    {
        c.BackColor = back;
        c.ForeColor = fore;
        foreach (Control child in c.Controls)
        {
            if (child is TextBox or ComboBox)
            {
                child.BackColor = Color.White;
                child.ForeColor = Color.Black;
            }
            else ApplyColors(child, back, fore);
        }
    }
}
