Care Package Manager

Let's you change what care packages can contain. 让你能改变补给包包含什么。

Features:
- increase selection of care package up to 6 (visibility limited by screen resolution)
- increase selection of duplicants up to 6 (visibility limited by screen resolution)
- bonus attribute chance; increase overall attribute sum (negative values reduce attributes)
- option to get always 3 interests
- option to remove starter conditions (some dupes cannot normally show up)
- option to have duplicants reshuffle button even after game start
- option to have duplicants and care packages ordered (only works with DLC enabled)
- multiplier for care package amounts

Manual editing only:
- remove or add new entries to possible care packages; this can be anything, as long as the game recognizes the given "Tag"
- add new restrictions onlyAfterCycle or onlyUntilCycle to get specific packages on specific cycles
NOTE: Make sure that there are always at least 2 valid entries, otherwise it will crash when accessing the portal

Config file found here: (after enabling it)
%userprofile%\documents\Klei\OxygenNotIncluded\mods\Care Package Manager.json

On MacOS: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/Care Package Manager.json
On Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/Care Package Manager.json

Manual download here:
[Github](https://github.com/Truinto/ONI-Modloader-SimpleMods/tree/master/Mods/CarePackageMod)

Tip: oni-assistant.com
If you open something it will show its internal name in the URL

______________________________________
Translation not up to date. Feel free to submit your own.

让你能改变补给包包含什么。

默认：
- 删除所有刷新物品的周期限制
- 增加少量太空材料和振荡器充电器

可以在配置文件中自定义的：
- 删除现有的刷新物品
- 改变刷新物品的数量
- 设置自定义周期限制
- 添加物品; 游戏必须识别“标签”！
- 将刷新列表扩充到6
- rosterIsOrdered 可以将刷新列表排序，复制人在左边，补给包在右边
- onlyUntilCycle 可以让某些补给包在固定周期后不再出现

注意：请确保至少有2个物品在周期0时有效，否则将因为没东西可刷崩溃。

配置文件路径: (after enabling it)
%userprofile%\documents\Klei\OxygenNotIncluded\mods\Care Package Manager.json

如果上面那个找不到试试这个:
MacOS: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/Care Package Manager.json

翻译一下作者对标签(Tag)的描述:

标签指的是游戏内部为每个物品创建的唯一标识符号.如果想要添加物品,则必须填入物品在游戏中的唯一标识符号(标签).

遗憾的是,目前没有公开的标签列表,因此如果想要添加物品,需要通过反编译的手段.

如果你觉得寻找标签的过程中有困难,可以联系作者协助.