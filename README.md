🌏 中文 / [english](./ReadMe-en.md)

# 恐怖黎明任务和对话文本提取工具

## 项目介绍
这是一个使用.NET 8和WinForms开发的工具，用于提取恐怖黎明(Grim Dawn)游戏中的任务文本和对话文本。
- 本项目参考Carson_N的powershell脚本而来[初始来源](https://forums.crateentertainment.com/t/japanese-community-of-the-rot/128705/71)

## 功能特性
- 提取.qst和.cnv文件中的文本内容并转换为txt文件
- 支持多种输出方式：
  - 输出至原目录下同名txt文件
  - 输出至所选目录下的CnvQstToTxtNoLevel目录（所有txt在同一目录）建议使用此项，这也是默认项
  - 输出至所选目录下的CnvQstToTxtKeepLevel目录（保留目录结构）



## 系统要求
- Windows操作系统
- .NET 8.0运行时

## 使用方法
1. 浏览或拖放一个包含.qst和.cnv文件的目录
2. 选择输出方式
3. 点击开始按钮
4. 等待处理完成，查看日志输出

## 注意事项
- 请确保目录路径正确
- 处理大量文件时可能需要一些时间
- 如有问题请提交issues

## 示例-翻译恐怖黎明完整游戏或MOD的方法
1. 将text_en.arc解包，翻译=后面的内容
2. 将Conversations.arc 和 Quests.arc解包，（可以使用我发布的另一款工具[arzedit-GUI](https://github.com/zhangqi222/arzedit-GUI)）
3. 使用GrimDawnCnvQstToTXT将解包后的内容生成TXT
4. 将生成的TXT翻译为你想要的语言，注意：翻译时不要修改文件名，不要修改文件中的行，结尾空行也不能删除（只翻译，不要做其它的）
5. 将翻译后的TXT放在text_zh文件夹（此处拿中文做示例）
6. 将翻译后的text_en中的内容也放在text_zh文件夹
7. 使用[arzedit-GUI](https://github.com/zhangqi222/arzedit-GUI)打包为text_zh.arc
8. 将text_zh.arc放在resources目录下即可