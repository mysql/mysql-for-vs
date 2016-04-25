/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_COLLECTION_CREATE_INDEX_H_
#define _MOD_COLLECTION_CREATE_INDEX_H_

#include "dynamic_object.h"

namespace mysh
{
  namespace mysqlx
  {
    class Collection;

    /**
    * Handler for index creation on a Collection.
    *
    * This object provides the necessary functions to allow adding an index into a collection.
    *
    * This object should only be created by calling any of the createIndex functions on the collection object where the index will be created.
    *
    * \sa Collection
    */
    class CollectionCreateIndex : public Dynamic_object, public boost::enable_shared_from_this<CollectionCreateIndex>
    {
    public:
      CollectionCreateIndex(boost::shared_ptr<Collection> owner);

      virtual std::string class_name() const { return "CollectionCreateIndex"; }

      shcore::Value create_index(const shcore::Argument_list &args);
      shcore::Value field(const shcore::Argument_list &args);
      virtual shcore::Value execute(const shcore::Argument_list &args);

#ifdef DOXYGEN
      CollectionCreateIndex createIndex(String name);
      CollectionCreateIndex createIndex(String name, IndexType type);
      CollectionCreateIndex field(DocPath documentPath, IndexColumnType type, Bool isRequired);
      Result execute();
#endif

    private:
      boost::weak_ptr<Collection> _owner;
      shcore::Argument_list _create_index_args;
    };
  }
}

#endif
