/*
   Copyright (c) 2015, 2016 Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _NGS_BUFFER_H_
#define _NGS_BUFFER_H_

#include <stdint.h>
#include <list>
#include <vector>
#include <boost/core/noncopyable.hpp>
#include <boost/asio/buffer.hpp>
#include "ngs/protocol/page_pool.h"

namespace ngs
{

  enum Alloc_result{ Memory_allocated, Memory_error, Memory_no_free_pages };

  class Buffer : private boost::noncopyable
  {
  public:
    typedef Resource<Page>           Buffer_page;
    typedef std::list< Buffer_page > Page_list;

    Buffer(Page_pool& page_pool);

    virtual ~Buffer();

    Alloc_result reserve(size_t space);
    Alloc_result add_pages(unsigned int npages);

    bool uint32_at(size_t offset, uint32_t &ret);
    bool int32_at(size_t offset, int32_t &ret);
    bool int8_at(size_t offset, int8_t &ret);

    size_t capacity() const;
    size_t length() const;
    size_t available_space() const;

    Page_list &pages() { return m_pages; }
    std::vector<boost::asio::mutable_buffer> get_asio_buffer();
    void add_bytes_transferred(size_t nbytes);

    Resource<Page> pop_front();
    void  push_back(const Resource<Page> &);

  protected:
    size_t m_capacity;
    size_t m_length;
    Page_pool& m_page_pool;
    Page_list m_pages;
  };

} // namespace ngs

#endif // _NGS_BUFFER_H_
