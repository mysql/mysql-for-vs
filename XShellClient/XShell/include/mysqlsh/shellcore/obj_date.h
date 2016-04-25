/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#include "shellcore/types_cpp.h"

#ifndef _SHCORE_OBJ_DATE_H_
#define _SHCORE_OBJ_DATE_H_

namespace shcore
{
  class SHCORE_PUBLIC Date : public Cpp_object_bridge
  {
  public:
    Date(int year, int month, int day, int hour, int min, float sec);

    virtual std::string class_name() const { return "Date"; }

    virtual std::string &append_descr(std::string &s_out, int indent = -1, int quote_strings = 0) const;
    virtual std::string &append_repr(std::string &s_out) const;
    virtual void append_json(shcore::JSON_dumper& dumper) const;

    virtual std::vector<std::string> get_members() const;
    virtual Value get_member(const std::string &prop) const;
    virtual void set_member(const std::string &prop, Value value);

    virtual bool operator == (const Object_bridge &other) const;
    bool operator == (const Date &other) const;

    int64_t as_ms() const;

    int get_year() const { return _year; }
    int get_month() const { return _month; }
    int get_day() const { return _day; }
    int get_hour() const { return _hour; }
    int get_min() const { return _min; }
    float get_sec() const { return _sec; }

  public:
    static Object_bridge_ref create(const shcore::Argument_list &args);
    static Object_bridge_ref unrepr(const std::string &s);
    static Object_bridge_ref from_ms(int64_t ms_since_epoch);

  private:
    int _year;
    int _month;
    int _day;
    int _hour;
    int _min;
    float _sec;
  };
}

#endif
