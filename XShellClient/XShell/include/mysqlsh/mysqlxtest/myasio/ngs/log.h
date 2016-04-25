/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/


#ifndef _NGS_LOG_H_
#define _NGS_LOG_H_

#ifndef NGS_DISABLE_LOGGING

# ifdef WITH_LOGGER
#  include <logger/logger.h>
# else
#  include "xpl_log.h"
# endif

#else

#define log_debug(...) do {} while(0)
#define log_info(...) do {} while(0)
#define log_warning(...) do {} while(0)
#define log_error(...) do {} while(0)

#endif

#endif
