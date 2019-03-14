# CSharp_vs_CPP
Here I present both C# and C++ code which do the same thing: constructing Chinese-English dictionary out of text file.
C# code is taken from Rico Mariani's article (https://blogs.msdn.microsoft.com/ricom/2005/05/10/performance-quiz-6-chineseenglish-dictionary-reader/), fixed and improved. 

In his article, Rico ports Raymond Chen's code, which has been optimized through a series of articles, to .NET, than he optimizes it and compares its performance with native C++ version. At the end of the article he demonstrates that even raw C# port of Raymond's initial code performs better than his optimized version, let alone optimized C# version. And so one might get an impression that C# is faster than C++. Especially when it’s taken into account how much effort went into optimizing C++. Since Raymond’s C++ code at the end of his series became grossly overcomplicated compared to one from initial point, one might get wrong opinion on C/C++, and that using C/C++ is just not worth the effort. But that's not true. Optimized C++ is actually very simple. It's just that Raymond went in a wrong direction with his optimization strategy.

C++ code given here is not the one from Raymond’s article, but rather port of optimized Rico's .NET code.

Dictionary downloaded from:
https://www.mdbg.net/chinese/export/cedict/cedict_1_0_ts_utf-8_mdbg.zip
