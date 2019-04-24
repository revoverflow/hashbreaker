<p align="center">
    <img src="https://cdn.discordapp.com/attachments/569638516934311958/570634517077950517/hb.png" height="100" width="500"/>
    <br/>
    <img src="https://img.shields.io/github/last-commit/dotOverflow/hashbreaker.svg?style=for-the-badge"/>
    <img src="https://img.shields.io/github/repo-size/dotOverflow/hashbreaker.svg?style=for-the-badge"/>
    <img src="https://img.shields.io/github/downloads/dotOverflow/hashbreaker/total.svg?style=for-the-badge"/>
    <img src="https://img.shields.io/github/stars/dotOverflow/hashbreaker.svg?style=for-the-badge"/>
</p>

#

## __What's HashBreaker ?__
HashBreaker is an C# hash tool program to easily crypt and decrypt in many different hashing algorithms like MD5, SHA-1, SHA-256 or ODO (our own hashing algorithm).

## __Ok, but an hash is not reversible...__
Yes, that's real, but we use a dictionnary bruteforce attack, fully optimised for cracking any hash.

## __Ok, but why i should use that ?__
HashBreaker can be used to crypt some confidential data like passwords or to decrypt database data.

## __How can i install this ?__
There is two methods to install HashBreaker :
### **- Using a public release (recommended) :**
This is the simpliest method (and the recommended one) :

You just need to go to the [repo's releases tab]("https://github.com/dotOverflow/hashbreaker/releases"), then download the latest version of **HashBreaker**. You'll get the built .exe file, just start it and you're ready !

### **- Build :**
This method is adapted for developers, if you're not familiar to Visual Studio, use the first method.

First, download the source code using git command line or by downloading the zip file :

```$ git clone https://github.com/dotOverflow/hashbreaker/```

Then, open the folder with Visual Studio and build the project, here is a tutorial for Building with Visual Studio 2019 :

https://docs.microsoft.com/en-us/visualstudio/ide/compiling-and-building-in-visual-studio?view=vs-2019