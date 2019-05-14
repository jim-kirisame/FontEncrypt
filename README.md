# FontEncrypt

一个将字体打乱顺序的软件。将字体内的字符重映射，达到加密文档的目的。

相关博客: [使用字型替换大法来加密word文档](https://www.lotlab.org/2019/05/14/使用字型替换大法来加密word文档)

# 用法

### 处理字体

```
ttx simsun.ttc -y 0 # 将宋体转换为ttx格式
fontencrypt.exe font simsun.ttx # 打乱字体
ttx -o simsun_new.ttf simsun.ttc # 将ttx转换回去
```

### 加解密文本

```
fontencrypt.exe text # 加密
fontencrypt.exe text decode # 解密
```

## License

GPL v3+