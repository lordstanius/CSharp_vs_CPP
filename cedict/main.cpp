#include <windows.h>
#include <vector>

struct DictionaryEntry
{
    wchar_t* m_trad;
    wchar_t* m_pinyin;
    wchar_t* m_english;
    bool Parse(wchar_t* buff, size_t ibIn, size_t ibMax, size_t* ibOut);
};

bool DictionaryEntry::Parse(wchar_t* buff, size_t ibIn, size_t ibMax, size_t* ibOut)
{
    int state = 0;
    char chTrans = ' ';
    int start = ibIn;
    int end = 0;
    int i;

    for (i = ibIn; buff[i] != '\n' && i < ibMax; i++)
    {
        if (buff[i] != chTrans)
            continue;

        switch (state)
        {
        case 0:
            m_trad = &buff[start];
            buff[i] = 0;
            state = 1;
            chTrans = '[';
            break;

        case 1:
            start = i + 1;
            chTrans = ']';
            state = 2;
            break;

        case 2:
            m_pinyin = &buff[start];
            buff[i] = 0;
            chTrans = '/';
            state = 3;
            break;

        case 3:
            start = i + 1;
            state = 4;
            break;

        case 4:
            end = i;
            state = 5;
            break;

        case 5:
            end = i;
            break;
        }
    }

    *ibOut = i;
    if (state == 5)
    {
        m_english = &buff[start];
        buff[end] = 0;
        return true;
    }
    return false;
}

class Dictionary
{
public:
    Dictionary();
    ~Dictionary();
    int Length() { return v.size(); }
    const DictionaryEntry& Item(int i) { return v[i]; }
private:
    std::vector<DictionaryEntry> v;
    wchar_t* m_buff;
};

#define FILESIZE 9 * 1024 * 1024

Dictionary::Dictionary()
{
    FILE* src = fopen("cedict_ts.u8", "rt, ccs=UTF-8");
    m_buff = new wchar_t[FILESIZE];
    size_t count = fread(m_buff, sizeof(wchar_t), FILESIZE, src);
    fclose(src);

    size_t ib = 0;
    while (ib < count) {
        if (m_buff[ib] == '#') {
            // there must still be a newline or we wouldn't be here
            while (m_buff[ib] != '\n')
                ib++;
        }
        else {
            DictionaryEntry de;
            if (de.Parse(m_buff, ib, count, &ib))
                v.push_back(de);
        }
        ib++;
    }
}

Dictionary::~Dictionary()
{
    delete[] m_buff;
}

int __cdecl main(int argc, const char* argv[])
{
    LARGE_INTEGER freq, startTime, endTime;
    QueryPerformanceFrequency(&freq);
    QueryPerformanceCounter(&startTime);

    Dictionary dict; // ~50 ms (release)

    QueryPerformanceCounter(&endTime);

    printf("Length: %d\n", dict.Length());
    printf("frequency: %0d\n", freq.QuadPart);
    printf("time: %.5f s", (endTime.QuadPart - startTime.QuadPart) / (double)freq.QuadPart);
    return 0;
}
