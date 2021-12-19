// Thanks to atom0s
// https://atom0s.com/forums/viewtopic.php?f=21&t=151&sid=d2fd50008534f41bbc3ab1a8b1ef6a6e

#pragma once

struct NostaleStringA
{
    char*   m_Buffer;
    size_t  m_Length;
 
    NostaleStringA(void)
        : m_Buffer(nullptr)
        , m_Length(0)
    { }
    NostaleStringA(const char* str)
        : m_Buffer(nullptr)
        , m_Length(0)
    {
        this->set(str);
    }
    ~NostaleStringA(void)
    {
        if (this->m_Buffer != nullptr)
            delete[] this->m_Buffer;
        this->m_Buffer = nullptr;
    }
 
    // Returns the size of the string.
    size_t len(void)
    {
        return (this->m_Buffer != nullptr) ? this->m_Length : 0;
    }
 
    // Returns the string within the buffer.
    char* get(void)
    {
        return (this->m_Buffer != nullptr) ? (char*)(this->m_Buffer + 0x08) : nullptr;
    }
 
    // Sets the string buffer.
    void set(const char* str)
    {
        if (this->m_Buffer != nullptr)
            delete[] this->m_Buffer;
 
        this->m_Length = strlen(str);
        this->m_Buffer = new char[this->m_Length + 8 + 1];
 
        *(unsigned int*)(this->m_Buffer + 0x00) = 1; // Reference Count
        *(unsigned int*)(this->m_Buffer + 0x04) = this->m_Length; // Length
        memcpy(this->m_Buffer + 0x08, str, this->m_Length);
        this->m_Buffer[this->m_Length + 0x08] = '\0';
    }
};
 
struct NostaleStringW
{
    wchar_t*    m_Buffer;
    size_t      m_Length;
 
    NostaleStringW(void)
        : m_Buffer(nullptr)
        , m_Length(0)
    { }
    NostaleStringW(const wchar_t* str)
        : m_Buffer(nullptr)
        , m_Length(0)
    {
        this->set(str);
    }
    ~NostaleStringW(void)
    {
        if (this->m_Buffer != nullptr)
            delete[] this->m_Buffer;
        this->m_Buffer = nullptr;
    }
 
    // Returns the size of the string.
    size_t len(void)
    {
        return (this->m_Buffer != nullptr) ? this->m_Length : 0;
    }
 
    // Returns the string within the buffer.
    wchar_t* get(void)
    {
        return (this->m_Buffer != nullptr) ? (wchar_t*)((char*)this->m_Buffer + 0x08) : nullptr;
    }
 
    // Sets the string buffer.
    void set(const wchar_t* str)
    {
        if (this->m_Buffer != nullptr)
            delete[] this->m_Buffer;
 
        this->m_Length = wcslen(str) * 2;
        this->m_Buffer = new wchar_t[this->m_Length + 8 + 1];
 
        *(unsigned int*)((char*)this->m_Buffer + 0x00) = 1; // Reference Count
        *(unsigned int*)((char*)this->m_Buffer + 0x04) = this->m_Length; // Length
        memcpy((char*)this->m_Buffer + 0x08, str, this->m_Length);
        *(wchar_t*)((char*)this->m_Buffer + this->m_Length + 0x08) = L'\0';
    }
};