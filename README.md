### Masuit.MyBlogs
<a href="https://gitee.com/masuit_admin/Masuit.MyBlogs"><img src="https://gitee.com/static/images/logo-black.svg" height="32"></a> <a href="https://github.com/ldqk/Masuit.MyBlogs"><img src="https://p.pstatp.com/origin/13841000102b8e2ba20b2" height="32"></a>  
masuit.com个人博客站项目源码，高性能低占用的博客系统，这也许是我写过的性能最高的web项目了。目前日均处理请求数80-120w次，同时在线活跃用户数30-100人，数据量累计已达到100多万条，数据库+Redis+网站主程序同时运行在一台2核2GB的机器上，浏览器页面请求秒级响应，CPU平均使用率控制在20%左右。  
![任务管理器](https://i.loli.net/2020/02/19/cjaxrWlLVvSAF8H.png)
### 演示站点
[https://masuit.com](https://masuit.com)

[![LICENSE](https://img.shields.io/badge/license-Anti%20996-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)

请注意：一旦使用本开源项目以及引用了本项目或包含本项目代码的公司因为违反劳动法（包括但不限定非法裁员、超时用工、雇佣童工等）在任何法律诉讼中败诉的，项目作者有权利追讨本项目的使用费，或者直接不允许使用任何包含本项目的源代码！

## Stargazers over time  
<img src="https://starchart.cc/ldqk/Masuit.MyBlogs.svg">    

### 前端请求支援
目前网站前端页面的代码比较零乱，到处都是，大家想吐槽的尽管吐槽吧，也想找个人帮忙设计下整体的前端页面，有兴趣愿意贡献代码的的小伙伴，欢迎Pull Request吧！😂😂
### 开发环境
操作系统：Windows 10 2004  
IDE：Visual Studio 2019 Enterprise 16.5  
数据库：SQL Server 2017/MySQL 8.0  
Redis：redis-server-windows 3.2  
运行时：必须是.NET Core 3.1 
### 当前运行环境
操作系统：Windows Server 2008 R2/Linux+docker  
数据库：SQL Server 2012 express/MySQL 8.0  
Redis：redis-server-windows 3.2  
运行时：.NET Framework 4.7.2/.NET Core 3.1  
服务器配置：2核+4GB+1M
`请勿使用阿里云、百度云等活动超卖机运行本程序，否则卡出翔！！！`  
`如何判断服务器商是否有超卖：给你的服务器跑个分，如果跑分接近于网络上该处理器公布的分数，则不是超卖的机器，计算公式：总分/核心数进行比较，由于是虚拟机，如果单独比较单核跑分，没有参考意义`
### 硬件要求
||最低配置|推荐配置|豪华配置|
| --------   | -----:   | :----: | :----: |
|CPU|1核|2核|4核|
|内存|1GB|2GB|4GB|
|带宽|1Mbps|1Mbps|5Mbps|
|数据库|SQL Server 2008/MySQL 5|SQL Server 2012/MySQL 8|SQL Server 2016/MySQL 8|
### 主要功能
#### 服务器性能监控
可直接在线实时监控服务器的运行状态，包括CPU、网络带宽、磁盘使用率、内存占用等情况，可记录最近一天的服务器健康状态，通过websocket进行数据的推送，仅支持Windows，且需要Windows安装最新的更新。
#### 文章管理
- 包含文章审核、文章合并、文章列表的增删查改、分类管理、专题管理；
- 文章审核：当用户在前台页进行投稿后，会进入审核状态，审核通过后，才会在前台页的文章列表中展示出来。
- 文章合并：当用户在前台页进行了文章的编辑后，会创建出文章的合并请求，当后台管理进行相应的合并操作后，前台用户的修改才会正式生效，可以直接合并、编辑并合并和拒绝合并，拒绝时，修改人会收到相应的邮件通知。
- 文章操作：可对文章进行修改、新增、置顶、临时删除(下架)、还原、永久删除、禁止评论等操作，编辑后的文章会生成历史版本。
- 分类管理：对文章的分类进行增删查改和文章的移动等操作，与文章的关系：一对多。
- 专题管理：对文章的专题进行管理，与文章的关系：多对多。
- 快速分享：首页快速分享栏目的管理。
#### 评论和留言管理
对前台用户提交的留言和评论进行审核，当前台用户提交的内容可能包含有敏感词时，会进入人工审核，审核成功才会在前台页中展示。
#### 消息通知
站内消息包含评论、留言、投稿、文章合并等通知。
#### 公告管理
对网站的公告进行增删查改管理。
#### 杂项页管理
一些通用的页面管理，可自由灵活的创建静态页面。
#### 系统设置
- 包含系统的全局设置、防火墙管理、网站运行日志记录、友链管理、邮件模板的管理。
- 全局设置：网站的一些基本配置和SEO相关操作等；
- 防火墙：对网站的所有请求进行全局流量的拦截，让规则内的请求阻止掉，支持黑名单、白名单、IP地址段、国家或地区、关键词审查等规则；
#### 广告管理
主动式的广告投放管理，支持竞价排名，支持在banner、边栏、页内、列表内的广告展示，竞价或权重的高低决定广告出现的概率。
#### 赞助管理
对网站打赏进行增删查改操作，自动掩码。
#### 搜索统计
当前台用户每Session周期内的关键词搜索，不重复的关键词将会被记录，用于热词统计，仅记录最近一个月内的所有搜索关键词，用于统计当月、7天以及当天的搜索热词。
#### 任务管理
hangfire的可视化管理页面
#### 文件管理
服务器文件的在线管理，支持浏览、预览、压缩、解压缩、创建文件夹、上传、下载、打包下载等文件的基本操作。

### 项目架构
- 项目采用单体架构，方便部署和配置，传统的MVC模式，ASP.NET Core MVC+EF Core的简单架构。  
- Controller→Service→Repository→DbContext  
![](https://git.imweb.io/ldqk/imgbed/raw/master/5ccbcc714c3db.jpg)  
### 项目文件夹定义：
App_Data：存放网站的一些常规数据，以文本的形式存在，这类数据不需要频繁更新的。  
┠─cert文件夹：存放https证书  
┠─ban.txt：敏感词库  
┠─CustomKeywords.txt：搜索分词词库  
┠─denyip.txt：IP地址黑名单  
┠─DenyIPRange.txt：IP地址段黑名单  
┠─GeoLite2-City.mmdb：MaxMind地址库  
┠─ip2region.db：ip2region地址库  
┠─mod.txt：审查词库  
┠─whitelist.txt：IP地址白名单  
Common：之前老项目的Common项目；  
Configs：项目的一些配置对象  
Controllers：控制器  
Extensions：一些扩展类或一些项目的扩展功能，比如hangfire、ueditor、中间件、拦截器等；  
Hubs：SignalR推送服务类；  
Infrastructure：数据访问基础设施，包含Repository和Services，相当于老项目的DAL和BLL；  
Migrations：数据库CodeFirst模式的迁移文件；  
Models：老项目的Models项目，存放一些实体类或DTO；  
Views：razor视图  
wwwroot：项目的所有静态资源；  
### 核心功能点技术实现
#### 后端技术栈：
依赖注入容器：.NET Core自带的+Autofac，autofac主要负责批量注入和属性注入；  
实体映射框架：automapper 9.0；  
缓存框架：CacheManager统一管理网站的热数据，如Session、内存缓存，EFSecondLevelCache.Core负责管理EF Core的二级缓存；  
定时任务：hangfire统一管理定时任务，包含友链回链检查、文章定时发布、访客统计、搜索热词统计、Lucene库刷新等任务；  
Websocket：SignalR进行流推送实现服务器硬件健康状态的实时监控；  
硬件检测：Masuit.Tools封装的硬件检测功能；  
全文检索：Masuit.LuceneEFCore.SearchEngine基于Lucene.Net 4.8实现的全文检索中间件；  
中文分词：结巴分词结合本地词库实现中文分词；  
断点下载：Masuit.Tools封装的断点续传功能；  
Redis：CSRedis负责Redis的读写操作；  
文件压缩：Masuit.Tools封装的zip文件压缩功能；  
Html字符串操作：htmldiff.net-core实现文章版本的内容对比，HtmlAgilityPack实现html字符串的“DOM”操作，主要是用于提取img标签，HtmlSanitizer实现表单的html代码的仿XSS处理；  
图床：支持多个图床的上传：gitee、gitlab、阿里云OSS、sm.ms图床、人民网图床；  
拦截器：授权拦截器、请求拦截器负责网站全局流量的拦截和清洗、防火墙拦截器负责拦截网站自带防火墙规则的请求流量、异常拦截器、url重定向重写拦截器，主要用于将http的请求重定向到https；  
请求IP来源检查：IP2Region+本地数据库实现请求IP的来源检查；  
RSS：WilderMinds.RssSyndication实现网站的RSS源；  
EF扩展功能：zzzproject相关nuget包  
Word文档转换：OpenXml实现浏览器端上传Word文档转换为html字符串。  
在线文件管理：angular-filemanager+文件管理代码实现服务器文件的在线管理  
#### 前端技术栈
##### 前台页面：
基于bootstrap3布局  
ueditor+layedit富文本编辑器  
notie提示栏+sweetyalert弹窗+layui组件  
angularjs  

##### 后台管理页：
- angularjs单一页面应用程序  
- material布局风格  
- highchart+echart图表组件  
- ng-table表格插件  
- material风格angular-filemanager文件管理器  
#### 性能和安全相关
- hangfire实现分布式任务调度；
- Z.EntityFramework.Plus实现数据访问层的高性能数据库批量操作；
- Lucene.NET实现高性能站内检索；
- 通过url的敏感词检查过滤恶意流量；
- 限制客户端的请求频次；
- 表单的AntiForgeryToken防止恶意提交；
- ip2region+MaxMind地址库实现请求来源审查；
- 用户信息采用端到端RSA非对称加密进行数据传输；
### 项目部署
#### 编译：
编译需要将[Masuit.Tools](https://github.com/ldqk/Masuit.Tools)项目和[Masuit.LuceneEFCore.SearchEngine](https://github.com/ldqk/Masuit.LuceneEFCore.SearchEngine)项目也一起clone下来，和本项目平级目录存放，才能正常编译，否则，将[Masuit.Tools](https://github.com/ldqk/Masuit.Tools)项目和[Masuit.LuceneEFCore.SearchEngine](https://github.com/ldqk/Masuit.LuceneEFCore.SearchEngine)项目移除，通过nuget安装也是可以的。  
![](https://git.imweb.io/ldqk/imgbed/raw/master/20191019/6370710386639200004363431.png)  
#### 配置文件：
主要需要配置的是https证书、数据库连接字符、redis、BaiduAK以及图床配置，其他配置均为可选项，不配置则表示不启用；
![](https://p.pstatp.com/origin/1381c000155b45481aeec)  
同时，BaiduAK参与了数据库的加密，如果你没有BaiduAK，自行到百度地图开放平台申请，`免费的`。  
如果你使用了CDN，需要配置TrueClientIPHeader选项为真实IP请求转发头，如cloudflare的叫CF-Connecting-IP。
如果Redis不在本机，需要在配置文件中的Redis节下配置，固定为Redis，值的格式：127.0.0.1:6379,allowadmin=true，若未正确配置，将按默认值“127.0.0.1:6379,allowadmin=true,abortConnect=false”。  
IIS：部署时必须将应用程序池的标识设置为LocalSystem，否则无法监控服务器硬件，同时需要安装.NET Core Hosting运行时环境，IIS程序池改为无托管代码。  
![](https://git.imweb.io/ldqk/imgbed/raw/master/5ccbf30b6a083.jpg)  
独立运行：配置好环境和配置文件后，可直接通过dotnet Masuit.MyBlogs.Core.dll命令或直接双击Masuit.MyBlogs.Core.exe运行，也可以通过nssm挂在为Windows服务运行，或者你也可以尝试在Linux下部署。  
docker：自行爬文。  

### 后台管理：
- 初始用户名：masuit  
- 初始密码：123abc@#$

### 推荐项目
基于EntityFrameworkCore和Lucene.NET实现的全文检索搜索引擎：[Masuit.LuceneEFCore.SearchEngine](https://github.com/ldqk/Masuit.LuceneEFCore.SearchEngine "Masuit.LuceneEFCore.SearchEngine")

.NET万能框架工具库：[Masuit.Tools](https://github.com/ldqk/Masuit.Tools)
### 友情赞助
![打赏支持](https://ae01.alicdn.com/kf/H9c0ef439b7ae4a5ba4151456f3c5f0a2N.jpg)
