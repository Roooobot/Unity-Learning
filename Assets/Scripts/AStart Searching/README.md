A星寻路模块是根据Sebastian Lague大神在Youtube上的教学，虽然教学视频久远，但可以正常使用，可以根据需要进行修改。（具体的代码说明可以看他的教学视频，B站上也有）。
## 文件说明：
#### AStart Searching目录下的：
Grid：用于生成世界网格。
Line：用于在转弯处和终点处生成一条判定线，方便获取距离和位置。
Node：网格类，网格包含的内容。
Path：用于保存寻路路径
Pathfinding：寻路方法的实现
Unit：测试单元
#### Managers目录下的：
PathRequestManager：用于处理各对象的寻路请求
#### Tool目录下的：
Heap：最小堆的实现，用于openSet的优化
Singleton：单例模式
## 使用说明：
#### 1.场景中需要有一个空对象（A*）挂载3个脚本：Grid，Pathfinding 和 PathRequestManager。
Grid脚本：
displayGizmos ：设置是否开启网格绘制。
unwalkableMask：设置不可行走的图层，即障碍物图层。
gridWorldSize：设置需要生成网格的世界大小（以该对象为中心点）。
nodeRadius：设置每个网格的边长的一半的长度（网格中心到每个面的距离）。
walkableRegions：设置可行走区域的惩罚值（可以不设置）。具体操作：terrainMask：用于设置该图层；					terrainPenalty：设置该图层惩罚值。（数值越低，图层优先度越高，建议不小于0）。
#### 2.一个寻路对象，挂载1个脚本：Unit
Unit脚本：
target：设置寻路目标点。
turnSpeed：设置转弯速度。
turnDst：设置开始转弯距离。
speed：设置移动速度。
stoppingDst：设置开始减速距离（接近目标点会逐渐减速）。
