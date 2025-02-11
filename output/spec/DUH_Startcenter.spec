# -*- mode: python ; coding: utf-8 -*-


a = Analysis(
    ['D:/Projekte/Projekte_Python/duh_startcenter/NX_Startcenter.py'],
    pathex=[],
    binaries=[],
    datas=[('D:/Projekte/Projekte_Python/duh_startcenter/controller', 'controller/'), ('D:/Projekte/Projekte_Python/duh_startcenter/model', 'model/'), ('D:/Projekte/Projekte_Python/duh_startcenter/view', 'view/'), ('D:/Projekte/Projekte_Python/duh_startcenter/env.py', '.'), ('C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python312/Lib/site-packages/ttkbootstrap', 'ttkbootstrap/'), ('C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python312/Lib/site-packages/ttkcreator', 'ttkcreator/'), ('C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python312/Lib/tkinter', 'tkinter/'), ('C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python312/Lib/site-packages/PIL', 'PIL/')],
    hiddenimports=['PIL._imagingft', 'PIL._imaging'],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    noarchive=False,
    optimize=0,
)
pyz = PYZ(a.pure)

exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.datas,
    [],
    name='DUH_Startcenter',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    upx_exclude=[],
    runtime_tmpdir=None,
    console=False,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
    icon=['D:\\Projekte\\Projekte_Python\\duh_startcenter\\src\\images\\duhGroup_Logo.ico'],
)
