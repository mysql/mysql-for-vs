/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _NGS_MEMORY_H_
#define _NGS_MEMORY_H_

#include <boost/interprocess/smart_ptr/unique_ptr.hpp>
#include <boost/function.hpp>
#include <boost/none.hpp>

template <typename ArrayType>
void Memory_delete_array(ArrayType* array_ptr)
{
  delete[] array_ptr;
}

template <typename Type>
void Memory_delete(Type* ptr)
{
  delete ptr;
}

template<typename Type>
struct Memory_new
{
  typedef void (*functor_type)(Type *ptr);

  Memory_new()
  {
    function = Memory_delete<Type>;
  }

  Memory_new(const boost::none_t &)
  {
    function = NULL;
  }

  Memory_new(functor_type user_function)
  {
    function = user_function;
  }

  void operator() (Type *ptr)
  {
    if (function)
    {
      function(ptr);
    }
  }

  functor_type function;

  typedef boost::interprocess::unique_ptr<Type, Memory_new<Type> > Unique_ptr;
};

template <typename Type, typename DeleterType = boost::function<void (Type* Value_ptr)> >
struct Custom_allocator
{
  typedef boost::interprocess::unique_ptr<Type, DeleterType > Unique_ptr;
};

#endif // _NGS_MEMORY_H_
