Care Package Manager

Let's you change what care packages can contain. 让你能改变补给包包含什么。

Default:
- removes cycle restrictions from all packages
- adds small amounts of space materials and vacillator recharge

Things changeable in config:
- remove existing entries
- change quantities
- set custom cycle restriction
- add entries; game must recognizable the "Tag"!
- increase roster to 6
- rosterIsOrdered which makes dupes be on the left side, packages on the right
- onlyUntilCycle which will make a package unavailable after a certain cycle

NOTE: Make sure that there are at least 2 entries that are valid starting cycle 0, otherwise it will crash loading null.

Config file found here: (after enabling it)
%userprofile%\documents\Klei\OxygenNotIncluded\mods\Care Package Manager.json

On MacOS: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/Care Package Manager.json
On Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/Care Package Manager.json

Manual download here:
Github[github.com]

Tip: oni-assistant.com
If you open something it will show its internal name in the URL

______________________________________

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