/*
   Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/
#ifndef _OBJECT_FACTORY_H_
#define _OBJECT_FACTORY_H_

#include "shellcore/types_common.h"
#include "shellcore/types.h"

namespace shcore
{
  class SHCORE_PUBLIC Object_factory
  {
  public:
    typedef boost::shared_ptr<Object_bridge>(*Factory_function)(const Argument_list &args);

    //! Registers a metaclass
    static void register_factory(const std::string &package, const std::string &class_name,
                                 Factory_function function);

    static boost::shared_ptr<Object_bridge> call_constructor(const std::string &package, const std::string &name,
                                                             const Argument_list &args);

    static std::vector<std::string> package_names();
    static std::vector<std::string> package_contents(const std::string &package);

    static bool has_package(const std::string &package);
  };

#define REGISTER_OBJECT(M,O) shcore::Object_bridge_register<O>M ## _ ## O ## _register(#M,#O);
#define REGISTER_ALIASED_OBJECT(M,O,C) shcore::Object_bridge_register<C>M ## _ ## O ## _register(#M,#O);

  template<class ObjectBridgeClass>
  struct Object_bridge_register
  {
    Object_bridge_register(const std::string &module_name, const std::string &object_name)
    {
      shcore::Object_factory::register_factory(module_name, object_name, &ObjectBridgeClass::create);
    }
  };
};
#endif //_OBJECT_FACTORY_H_