# 牛子机器人-QQ机器人版
### 快速上手
#### 项目依赖
本项目依赖于任意一支持**正向WebSocket协议**的**OneBotV11**实现。

理论上该项目支持**Windows 10+,Linux,OS X**三个系统，受限于开发者条件，测试环境为**Linux/Ubuntu**，并且仅提供**Windows**和**Linux**的**AMD64**架构安装包。如果需要更多版本，可以自行编译或者前往QQ群:**745297798**询问了解。

Linux环境推荐使用：**NapCat**

Windows环境推荐使用：**LLOneBot**

**LLOneBot**安装更加简单，且有设置界面供小白轻松上手，但是需要服务器安装了**GUI环境**;

**NapCat**更加轻量，内存占用小，但是需要手动修改配置文件。

#### 启动及配置
前往**Releases**界面下载对应平台二进制编译文件，然后解压。解压后打开**config**文件夹里面的配置文件**main.json**，填写**WebSocket服务器ip**和**端口**，然后直接启动DickFighterBot主程序即可。

##### 配置文件说明

**ws_host**:WebSocket服务器ip

**port**:WebSocket服务器端口

**Interval**:消息处理的间隔，单位为毫秒(ms)，更长的间隔有助于避免被封号的概率

**FightEnergyCost**：每次斗牛消耗的体力值

**ExerciseEnergyCost**：每次锻炼牛子消耗的体力值

##### Linux环境下，执行：
```
chmod +x DickFighterBot
./DickFighterBot
```
