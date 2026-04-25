# RoadLike 开发笔记

## 项目概况
- Unity roguelike 驾驶游戏，用 URP 渲染管线
- GitHub: https://github.com/petershaoapp-jpg/RoadLike
- 团队成员：Peter（petershao666），Casey Masonek（老大），Jake
- 铁律：不擅自改动代码，一切改动需要 Peter 允许并详细汇报

## 项目结构
- 场景：TitleScreen, SampleScene（主场景）, LevelComplete
- 脚本目录：Assets/Scripts/
  - Controllers/：CarController.cs, BetterCarController.cs（Jake的WIP，不用）, ZombieController.cs, Boss1Controller.cs（类名是BossChargeController）
  - Data/：GameData.cs, PlayerData.cs, Upgrade.cs, Rarity.cs
  - UI/：HealthUI, NitroUI, Speedometer, TimerUI, UpgradeCard 等
  - Abstract Controllers/：IMovementController.cs 接口
  - Other Interfaces/：IDie.cs
- 数据：Assets/Data/ 下有 ScriptableObject 资产（GameData.asset, PlayerData.asset, Upgrades/）
- Prefabs：Zombie, Nitro charge, Upgrade Card

## 架构要点
- 移动系统：IMovementController 接口提供 GetMovement()，MoveConstantSpeed 组件读取并应用到 Rigidbody
- 生命系统：Health.cs（通用），PlayerDie.cs / EnemyDie.cs 实现 IDie 接口
- 升级系统：UpgradeManager / UpgradeSelector / UpgradeEffects，数据用 ScriptableObject
- 车用的是 WheelCollider 物理系统（CarController.cs）

## 本次 Session 完成的工作

### 1. Git 合并冲突解决
- 合并了 Casey 的新代码（Fangs 吸血升级、音效系统、新僵尸等）
- Gun.cs：合并了 Casey 的 health/audioSource/Fangs + Peter 的暴击系统/Lust/ShootRoutine 循环
- CarController.cs：按 Casey 版本为主，移除了 Peter 的 handling 升级功能
- SampleScene.unity：合并了 Casey 的新僵尸 + Peter 的 BossDemond

### 2. Boss1 开发（BossChargeController）
文件：Assets/Scripts/Controllers/Boss1Controller.cs（类名 BossChargeController）
场景物体名：BossDemond（Zombie prefab 放大到 5x5x5，maxHealth=100）

功能完整流程：
- 警戒范围：triggerDistance=35，玩家进入范围才触发
- PHASE 1 蓄力：停下来，显示红色危险区域，跟踪玩家方向（chargePrepTime=1.5s）
- PHASE 2 冲锋：销毁危险区域，锁定方向高速冲刺（chargeDuration=2s）
- 碰撞伤害：冲撞玩家扣血（chargeDamage=30）+ 撞飞效果（knockbackForce=50，建议调到15-20）
- 单次判定：_hasHitPlayer 防止一次冲锋多次扣血
- PHASE 3 休息：冲完原地停（restTime=3s），给玩家反打窗口

危险区域实现：
- 多段 Cube 拼接贴地（segmentLength=2m），每段独立 Raycast 找地面高度
- URP 透明材质运行时生成（Universal Render Pipeline/Lit, _Surface=1）
- 蓄力期间实时跟踪玩家方向

### 3. 已知问题 / 待调整
- knockbackForce=50 太夸张，建议调到 15-20
- BetterCarController.cs（Jake 的）不要挂在 Car 上，linearDamping=3 会让车很慢
- 危险区域贴地效果取决于地形 Collider，如果某些地方没 Collider 射线会打空

### 4. HellShooter 固定射击小怪
新增文件：
- Assets/Scripts/Controllers/HellShooterController.cs（类名 HellShooterController）
- Assets/Scripts/FireShot.cs（子弹脚本）

行为流程：
- 固定不动（GetMovement 返回零向量）
- 玩家进入 detectionRange（默认40）后开始转向瞄准
- 连射 burstCount（默认3）发子弹，每发间隔 timeBetweenShots（0.5s）
- 射完后休息 restTime（3s），此时玩家可冲上去撞死
- 循环上述流程

子弹（FireShot）：
- 运行时生成 Sphere，URP Lit 材质 + Emission 发光（默认橙红色）
- 直线飞行，碰到 Car 调 Health.TakeDamage，碰到任何东西销毁
- 5秒超时自毁

Unity 场景配置：
- 创建 Capsule → 命名 HellShooter，Tag 设 Enemy
- 挂组件：HellShooterController, MoveConstantSpeed, Health（maxHealth=100）, EnemyDie
- Rigidbody 勾 isKinematic
- 建子物体 FirePoint 放在胶囊前方，拖进 HellShooterController 的 firePoint 槽
- 需要给 EnemyDie 拖入 PlayerData 引用和 soul prefab（如果有 Gluttony 升级）

## 下一步建议
1. Boss 血条 UI（屏幕上方大血条）
2. Boss 多阶段行为（低血量时加速/缩短蓄力）
3. Boss 冲锋 miss 后硬直
4. Boss 死亡特殊处理（触发关卡完成/掉奖励）
5. HellShooter 数值调优（damage、射速、射程等需要实测调整）
