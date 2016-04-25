/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_CRUD_COLLECTION_DROP_INDEX_H_
#define _MOD_CRUD_COLLECTION_DROP_INDEX_H_

#include "dynamic_object.h"

namespace mysh
{
  namespace mysqlx
  {
    class Collection;

    /**
    * Handler for index dropping handler on a Collection.
    *
    * This object allows dropping an index from a collection.
    *
    * This object should only be created by calling the dropIndex function on the collection object where the index will be removed.
    *
    * \sa Collection
    */
    class CollectionDropIndex : public Dynamic_object, public boost::enable_shared_from_this<CollectionDropIndex>
    {
    public:
      CollectionDropIndex(boost::shared_ptr<Collection> owner);

      virtual std::string class_name() const { return "CollectionDropIndex"; }

      shcore::Value drop_index(const shcore::Argument_list &args);
      virtual shcore::Value execute(const shcore::Argument_list &args);

#ifdef DOXYGEN
      CollectionDropIndex dropIndex(String indexName);
      Result execute();
#endif

    private:
      boost::weak_ptr<Collection> _owner;
      shcore::Argument_list _drop_index_args;
    };
  }
}

#endif
